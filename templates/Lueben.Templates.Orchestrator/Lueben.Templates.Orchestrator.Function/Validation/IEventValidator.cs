using System.Threading.Tasks;
using Lueben.Microservice.EventHub;
using Newtonsoft.Json.Linq;

namespace Lueben.Templates.Orchestrator.Function.Validation
{
    public interface IEventValidator
    {
        public Task<Event<JObject>> GetValidatedEvent(string message);
    }
}