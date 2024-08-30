using System.Linq;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetAllApplications
{
    public class GetAllApplicationsQuerySql : IRequest<IQueryable<Application>>
    {
    }
}
