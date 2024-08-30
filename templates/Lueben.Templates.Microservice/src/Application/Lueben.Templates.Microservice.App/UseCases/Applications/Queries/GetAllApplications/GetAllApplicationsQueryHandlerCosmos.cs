using System.Linq;
using System.Threading.Tasks;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetAllApplications
{
    public class GetAllApplicationsQueryHandlerCosmos : GetAllEntitiesQueryHandlerCosmos<GetAllApplicationsQueryCosmos, Domain.Entities.Application>
    {
        public GetAllApplicationsQueryHandlerCosmos(IRepositoryCosmos<Application> applicationRepository) : base(applicationRepository)
        {
        }

        protected override async Task<IQueryable<Domain.Entities.Application>> GetEntitiesQuery(GetAllApplicationsQueryCosmos request)
        {
            var applicationCollection = await ApplicationRepository.GetAsync(application => application.Id != string.Empty);
            return applicationCollection.AsQueryable();
        }
    }
}
