using System;
using System.Threading;
using System.Threading.Tasks;
using Lueben.Microservice.Api.PipelineFunction.Constants;
using Lueben.Microservice.Api.PipelineFunction.Extensions;
using Lueben.Microservice.Api.PipelineFunction.Filters;
using Lueben.Microservice.Api.ValidationFunction.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Lueben.Templates.Microservice.Api.Filters
{
    public class RequiredHeadersFilter : FunctionBaseInvocationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequiredHeadersFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        private string SourceConsumer => _httpContextAccessor.HttpContext?.Request.GetHeaderValueOrDefault(Headers.SourceConsumer);

#pragma warning disable 618
        public override Task OnPipelineExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
#pragma warning restore 618
        {
            if (string.IsNullOrEmpty(SourceConsumer))
            {
                throw new ModelNotValidException($"{nameof(Headers.SourceConsumer)} header", $"'{Headers.SourceConsumer}' header is not set", "request headers");
            }

            return Task.CompletedTask;
        }
    }
}