using System.Linq;
using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases
{
    public abstract class GetAllEntitiesQueryHandlerCosmos<TQuery, TEntity> : IRequestHandler<TQuery, IQueryable<TEntity>>
        where TQuery : IRequest<IQueryable<TEntity>>
    {
        protected readonly IRepositoryCosmos<Application> ApplicationRepository;

        protected GetAllEntitiesQueryHandlerCosmos(IRepositoryCosmos<Application> applicationRepository)
        {
            ApplicationRepository = applicationRepository;
        }

        public async Task<IQueryable<TEntity>> Handle(TQuery request)
        {
            return await GetEntitiesQuery(request);
        }

        protected abstract Task<IQueryable<TEntity>> GetEntitiesQuery(TQuery request);
    }
}