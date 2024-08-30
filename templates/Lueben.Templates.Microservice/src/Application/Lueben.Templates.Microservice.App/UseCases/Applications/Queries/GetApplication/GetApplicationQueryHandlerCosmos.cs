using System.Linq;
using System.Threading.Tasks;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetApplication
{
    public class GetApplicationQueryHandlerCosmos : GetEntityQueryHandlerCosmos<GetApplicationQueryCosmos, Application>
    {
        public GetApplicationQueryHandlerCosmos(IRepositoryCosmos<Application> applicationRepository) : base(applicationRepository)
        {
        }

        protected override async Task<Application> GetEntityQuery(GetApplicationQueryCosmos request)
        {
            var applicationCollection = await ApplicationRepository.GetAsync(application => application.Id == request.Id);
            return applicationCollection.FirstOrDefault();
        }
    }
}
