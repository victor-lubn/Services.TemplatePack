using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lueben.Microservice.Api.Models;
using Lueben.Microservice.Diagnostics;
using Lueben.Microservice.RestSharpClient;
using Lueben.Microservice.RestSharpClient.Abstractions;
using Lueben.Templates.ApiClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace Lueben.Templates.ApiClient
{
    public class ApiClient : IApiClient
    {
        private const string UrlPath = "entity";
        private readonly ILogger<ApiClient> _logger;
        private readonly IRestSharpClient _restClient;
        private readonly IOptions<ApiOptions> _options;

        public ApiClient(ILogger<ApiClient> logger, IRestSharpClientFactory restSharpClientFactory, IOptions<ApiOptions> options)
        {
            Ensure.ArgumentNotNull(logger, nameof(logger));
            Ensure.ArgumentNotNull(restSharpClientFactory, nameof(restSharpClientFactory));
            Ensure.ArgumentNotNull(options, nameof(options));

            _restClient = restSharpClientFactory.Create(this);
            _options = options;
            _logger = logger;
        }

        public async Task<int> AddAsync(Entity entity)
        {
            var url = $"{_options.Value.BaseUrl}/{UrlPath}";

            try
            {
                var request = InitializeRequest(url, Method.Post);
                request.AddJsonBody(entity);
                var response = await _restClient.ExecuteRequestAsync<int>(request);

                return response;
            }
            catch (RestClientApiException ex)
            {
                ex.ResponseData = DeserializeErrorResponseData(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while posting object to API ({url})");

                throw;
            }
        }

        private static RestRequest InitializeRequest(string url,
            Method method,
            IReadOnlyList<object> parameters = null,
            IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            var uriService = new Uri(url);
            var request = new RestRequest(uriService.AbsoluteUri, method);
            request.PopulateUrlSegmentParameters(url, parameters);
            request.PopulateHeaders(headers);

            return request;
        }

        private ErrorResult DeserializeErrorResponseData(RestClientApiException ex)
        {
            if (ex.ResponseContent != null)
            {
                try
                {
                    var responseData = JsonConvert.DeserializeObject<ErrorResult>(ex.ResponseContent);
                    return responseData;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to deserialize error response content.");
                }
            }

            return null;
        }
    }
}