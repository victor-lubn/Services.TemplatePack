using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;

namespace Lueben.Templates.Microservice.App.UseCases
{
    public abstract class GetEntityQueryHandlerSql<TQuery, TEntity> : IRequestHandler<TQuery, TEntity>
        where TQuery : IRequest<TEntity>
    {
        protected readonly ILuebenContextSql LuebenContext;

        protected GetEntityQueryHandlerSql(ILuebenContextSql LuebenContext)
        {
            LuebenContext = LuebenContext;
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