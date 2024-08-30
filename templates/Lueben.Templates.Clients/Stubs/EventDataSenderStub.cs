using System.Collections.Generic;
using System.Threading.Tasks;
using Lueben.Microservice.EventHub;

namespace Lueben.Templates.Eventing.Clients.Stub.Stubs
{
    public class EventDataSenderStub : IEventDataSender
    {
        public Task SendEventAsync<T>(Event<T> data)
        {
            return Task.CompletedTask;
        }

        public Task SendEventsListAsync<T>(IEnumerable<Event<T>> data)
        {
            return Task.CompletedTask;
        }
    }
}