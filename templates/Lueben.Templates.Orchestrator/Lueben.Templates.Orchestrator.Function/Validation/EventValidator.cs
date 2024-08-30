using System;
using System.Linq;
using System.Threading.Tasks;
using Lueben.Microservice.EventHub;
using Lueben.Microservice.Mediator;
using Lueben.Microservice.Serialization;
using Lueben.Templates.Orchestrator.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lueben.Templates.Orchestrator.Function.Validation
{
    /// <summary>
    /// Example of event validator.
    /// </summary>
    public class EventValidator : IEventValidator
    {
        private readonly ILogger<EventValidator> _logger;
        private readonly IHandlerProvider _handlerProvider;

        public EventValidator(ILogger<EventValidator> logger, IHandlerProvider handlerProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
        }

        private bool IsNotValidEvent(Event<JObject> eventData)
        {
            if (eventData == null)
            {
                _logger.LogWarning("EventIncorrectModel");
                return true;
            }

            if (!EventHandlerExists(eventData.Type))
            {
                _logger.LogWarning("EventIncorrectType");
                return true;
            }

            return false;
        }

        private bool EventHandlerExists(string eventType)
        {
            var eventHandler = _handlerProvider.GetAllNotificationHandlers<Notification<Event<JObject>>>()
                .FirstOrDefault(h => (h as IEventHandler)?.IsApplicable(eventType) ?? false);
            if (eventHandler == null)
            {
                _logger.LogError($"No handler for event type: '{eventType}'");
            }

            return eventHandler != null;
        }

        private Task<Event<JObject>> GetValidatedData(string messageBody)
        {
            try
            {
                var eventData = JsonConvert.DeserializeObject<Event<JObject>>(messageBody, FunctionJsonSerializerSettingsProvider.CreateSerializerSettings());

                if (string.IsNullOrEmpty(eventData?.Type))
                {
                    _logger.LogError("Event type is not defined.");
                }
                else if (eventData.Data == null)
                {
                    _logger.LogError("Event data is not defined.");
                }
                else
                {
                    return Task.FromResult(eventData);
                }
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError(ex, "Failed to deserialize event.");
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, "Failed to deserialize event.");
            }

            return default;
        }

        public async Task<Event<JObject>> GetValidatedEvent(string message)
        {
            var eventData = await GetValidatedData(message);
            return IsNotValidEvent(eventData) ? default : eventData;
        }
    }
}