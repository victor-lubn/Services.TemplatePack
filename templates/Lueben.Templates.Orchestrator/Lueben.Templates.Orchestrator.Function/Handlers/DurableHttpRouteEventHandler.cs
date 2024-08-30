using System;
using System.Threading.Tasks;
using Lueben.Microservice.DurableFunction.Extensions;
using Lueben.Microservice.DurableHttpRouteFunction;
using Lueben.Microservice.EventHub;
using Lueben.Microservice.Serialization;
using Lueben.Templates.Orchestrator.Handlers;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lueben.Templates.Orchestrator.Function.Handlers
{
    /// <summary>
    /// Example of event handler.
    /// </summary>
    public class DurableHttpRouteEventHandler : EventMessageHandler
    {
        private readonly ILogger<DurableHttpRouteEventHandler> _logger;
        private readonly IOptionsSnapshot<HttpEventHandlerOptions> _options;
        private readonly IDurableClientFactory _durableClientFactory;
        private readonly IOptions<DurableTaskOptions> _durableTaskOptions;

        public DurableHttpRouteEventHandler(ILogger<DurableHttpRouteEventHandler> logger,
            IOptionsSnapshot<HttpEventHandlerOptions> options,
            IDurableClientFactory durableClientFactory,
            IOptions<DurableTaskOptions> durableTaskOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _durableClientFactory = durableClientFactory;
            _durableTaskOptions = durableTaskOptions;
        }

        public override bool IsApplicable(string eventType) => !string.IsNullOrEmpty(_options.Get(eventType)?.ServiceUrl);

        public override async Task ProcessEvent(Event<JObject> eventData)
        {
            var routeData = new HttpRouteInput
            {
                HandlerOptions = _options.Get(eventData.Type),
                Payload = JsonConvert.SerializeObject(eventData.Data, FunctionJsonSerializerSettingsProvider.CreateSerializerSettings())
            };

            var durableClient = _durableClientFactory.CreateClient(new DurableClientOptions
            {
                TaskHub = _durableTaskOptions.Value.HubName
            });

            var id = await durableClient.StartNewAsyncWithRetry(nameof(DurableHttpRouteFunction.Orchestrator), input: routeData);
            _logger.LogInformation($"Started workflow {id} to route {eventData.Type} event.");
        }
    }
}