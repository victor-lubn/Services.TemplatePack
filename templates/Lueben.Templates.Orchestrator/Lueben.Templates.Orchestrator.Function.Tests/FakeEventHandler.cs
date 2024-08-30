using System;
using System.Threading.Tasks;
using Lueben.Microservice.EventHub;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Orchestrator.Handlers;
using Newtonsoft.Json.Linq;

namespace Lueben.Templates.Orchestrator.Function.Tests
{
    public class FakeEventHandler : INotificationHandler<Notification<Event<JObject>>>, IEventHandler
    {
        private readonly string _eventType;

        public FakeEventHandler(string eventType)
        {
            _eventType = eventType;
        }

        public Task Handle(Notification<Event<JObject>> notification)
        {
            throw new NotImplementedException();
        }

        public bool IsApplicable(string eventType)
        {
            return eventType.Equals(_eventType);
        }

        public Task ProcessEvent(Event<JObject> eventData)
        {
            throw new NotImplementedException();
        }
    }
}