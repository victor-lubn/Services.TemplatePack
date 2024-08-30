using Lueben.Microservice.EntityFunction.Models;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetApplication
{
    public class GetApplicationQueryCosmos : IRequest<Application>, IEntityOperation<string>
    {
        public string Id { get; set; }
    }
}
