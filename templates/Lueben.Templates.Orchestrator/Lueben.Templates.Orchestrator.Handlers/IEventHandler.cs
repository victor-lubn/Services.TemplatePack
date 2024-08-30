using System.Threading.Tasks;
using Lueben.Microservice.EventHub;
using Newtonsoft.Json.Linq;

namespace Lueben.Templates.Orchestrator.Handlers
{
    public interface IEventHandler
    {
        public bool IsApplicable(string eventType);

        public Task ProcessEvent(Event<JObject> eventData);
    }
}