using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Commands.DeleteApplication
{
    public class DeleteApplicationCommandHandlerSql : IRequestHandler<DeleteApplicationCommandSql, Unit>
    {
        private readonly ILuebenContextSql _LuebenContext;

        public DeleteApplicationCommandHandlerSql(ILuebenContextSql LuebenContext)
        {
            _LuebenContext = LuebenContext;
        }

        public async Task<Unit> Handle(DeleteApplicationCommandSql request)
        {
            var application = await _LuebenContext.Application
                .FirstOrDefaultAsync(x => x.Id == request.Id).ConfigureAwait(false);

            if (application == null)
            {
                throw new EntityNotFoundException();
            }

            _LuebenContext.Application.Remove(application);
            await _LuebenContext.SaveChangesAsync();

            return Unit.Value;
        }
    }
}