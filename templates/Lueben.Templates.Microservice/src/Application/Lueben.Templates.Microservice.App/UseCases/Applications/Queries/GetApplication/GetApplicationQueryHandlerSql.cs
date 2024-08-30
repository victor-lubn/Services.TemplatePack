using System.Threading.Tasks;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetApplication
{
    public class GetApplicationQueryHandlerSql : GetEntityQueryHandlerSql<GetApplicationQuerySql, Application>
    {
        public GetApplicationQueryHandlerSql(ILuebenContextSql LuebenContext) : base(LuebenContext)
        {
        }

        protected override async Task<Application> GetEntityQuery(GetApplicationQuerySql request)
        {
            return await LuebenContext.Application
                .FirstOrDefaultAsync(x => x.Id == request.Id);
        }
    }
}