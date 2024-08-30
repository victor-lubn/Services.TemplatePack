using Lueben.Microservice.EntityFunction.Models;

namespace Lueben.Templates.Microservice.App.UseCases
{
    public abstract class TrackingCommandBase : IEntityOperation<string>
    {
        public string Id { get; set; }
    }
}
