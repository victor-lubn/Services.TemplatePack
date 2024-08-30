using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Lueben.Microservice.EventHub;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Orchestrator.Function.Validation;
using Lueben.Templates.Orchestrator.Handlers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Lueben.Templates.Orchestrator.Function
{
    public class OrchestratorFunction
    {
        private readonly ILogger<OrchestratorFunction> _logger;
        private readonly IMediator _mediator;
        private readonly IEventValidator _eventValidator;

        public OrchestratorFunction(ILogger<OrchestratorFunction> logger, IMediator mediator, IEventValidator eventValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventValidator = eventValidator ?? throw new ArgumentNullException(nameof(eventValidator));
        }

        [FunctionName(name: nameof(EventTrigger))]
        public async Task EventTrigger([EventHubTrigger(eventHubName: "%EventHubName%", Connection = "EventHubConnectionString")] EventData eventData)
        {
            var activity = eventData.ExtractActivity(typeof(OrchestratorFunction).FullName);
            activity.Start();

            try
            {
                var message = Encoding.UTF8.GetString(eventData.Body.ToArray());
                _logger.LogInformation("Received event: " + message);

                var appEvent = await _eventValidator.GetValidatedEvent(message);
                if (appEvent == null)
                {
                    return;
                }

                var notification = new Notification<Event<JObject>> { Data = appEvent };
                await _mediator.Publish(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process event.");
            }
            finally
            {
                activity.Stop();
            }
        }
    }
}