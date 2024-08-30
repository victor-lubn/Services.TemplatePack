using System;
using System.Threading.Tasks;
using Lueben.Microservice.EventHub;
using Lueben.Microservice.Mediator;
using Newtonsoft.Json.Linq;

namespace Lueben.Templates.Orchestrator.Handlers
{
    public abstract class EventMessageHandler : INotificationHandler<Notification<Event<JObject>>>, IEventHandler
    {
        public async Task Handle(Notification<Event<JObject>> eventMessage)
        {
            if (eventMessage == null)
            {
                throw new ArgumentNullException(nameof(eventMessage));
            }

            if (eventMessage.Data == null)
            {
                throw new ArgumentNullException(nameof(eventMessage.Data));
            }

            if (IsApplicable(eventMessage.Data.Type))
            {
                await ProcessEvent(eventMessage.Data);
            }
        }

        public abstract bool IsApplicable(string eventType);

        public abstract Task ProcessEvent(Event<JObject> eventData);
    }
}