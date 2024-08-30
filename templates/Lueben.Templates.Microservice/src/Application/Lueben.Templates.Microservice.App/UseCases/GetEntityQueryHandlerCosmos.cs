using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases
{
    public abstract class GetEntityQueryHandlerCosmos<TQuery, TEntity> : IRequestHandler<TQuery, TEntity>
        where TQuery : IRequest<TEntity>
    {
        protected readonly IRepositoryCosmos<Application> ApplicationRepository;

        protected GetEntityQueryHandlerCosmos(IRepositoryCosmos<Application> applicationRepository)
        {
            ApplicationRepository = applicationRepository;
        }

        public async Task<TEntity> Handle(TQuery request)
        {
            var entity = await GetEntityQuery(request);

            if (entity == null)
            {
                throw new EntityNotFoundException();
            }

            return entity;
        }

        protected abstract Task<TEntity> GetEntityQuery(TQuery request);
    }
}
