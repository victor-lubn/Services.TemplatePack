using System.Linq;
using System.Threading.Tasks;
using Lueben.Templates.Microservice.App.Infrastructure;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetAllApplications
{
    public class GetAllApplicationsQueryHandlerSql : GetAllEntitiesQueryHandlerSql<GetAllApplicationsQuerySql, Domain.Entities.Application>
    {
        public GetAllApplicationsQueryHandlerSql(ILuebenContextSql LuebenOnlineApplicationContext) : base(LuebenOnlineApplicationContext)
        {
        }

        protected override async Task<IQueryable<Domain.Entities.Application>> GetEntitiesQuery(GetAllApplicationsQuerySql request)
        {
            return await Task.FromResult(LuebenContext.Application.AsQueryable());
        }
    }
}