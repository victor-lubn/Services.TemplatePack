using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication
{
    public class PartiallyUpdateApplicationCommandHandlerSql : IRequestHandler<PartiallyUpdateApplicationCommandSql, Unit>
    {
        private readonly ILuebenContextSql _LuebenContext;
        private readonly IEntityFrameworkPatchSql _entityFrameworkPatch;

        public PartiallyUpdateApplicationCommandHandlerSql(ILuebenContextSql LuebenContext, IEntityFrameworkPatchSql entityFrameworkPatch)
        {
            _LuebenContext = LuebenContext;
            _entityFrameworkPatch = entityFrameworkPatch;
        }

        public async Task<Unit> Handle(PartiallyUpdateApplicationCommandSql request)
        {
            var application = await _LuebenContext.Application.FirstOrDefaultAsync(x => x.Id == request.Id).ConfigureAwait(false);
            if (application == null)
            {
                throw new EntityNotFoundException();
            }

            application.MarkUpdated();
            _LuebenContext.Application.Update(application);

            await _LuebenContext.SaveChangesAsync().ConfigureAwait(false);
            _LuebenContext.CleanTrackedEntity(application);

            await _entityFrameworkPatch.ApplyPatch<Application, PartiallyUpdateApplicationCommandSql>(request);

            return Unit.Value;
        }
    }
}