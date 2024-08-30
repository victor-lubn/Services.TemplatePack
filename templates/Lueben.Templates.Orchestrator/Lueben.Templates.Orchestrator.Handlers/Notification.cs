using Lueben.Microservice.Mediator;

namespace Lueben.Templates.Orchestrator.Handlers
{
    public class Notification<T> : INotification
    {
        public T Data { get; set; }
    }
}