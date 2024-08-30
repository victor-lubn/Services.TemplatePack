using System.Linq;
using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Infrastructure;

namespace Lueben.Templates.Microservice.App.UseCases
{
    public abstract class GetAllEntitiesQueryHandlerSql<TQuery, TEntity> : IRequestHandler<TQuery, IQueryable<TEntity>>
        where TQuery : IRequest<IQueryable<TEntity>>
    {
        protected readonly ILuebenContextSql LuebenContext;

        protected GetAllEntitiesQueryHandlerSql(ILuebenContextSql LuebenContext)
        {
            LuebenContext = LuebenContext;
        }

        public async Task<IQueryable<TEntity>> Handle(TQuery request)
        {
            return await GetEntitiesQuery(request);
        }

        protected abstract Task<IQueryable<TEntity>> GetEntitiesQuery(TQuery request);
    }
}