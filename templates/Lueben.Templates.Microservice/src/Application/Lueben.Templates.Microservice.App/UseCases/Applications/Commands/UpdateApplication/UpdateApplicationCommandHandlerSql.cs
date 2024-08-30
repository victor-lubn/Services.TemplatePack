using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication
{
    public class UpdateApplicationCommandHandlerSql : IRequestHandler<UpdateApplicationCommandSql, Unit>
    {
        private readonly ILuebenContextSql _LuebenContext;
        private readonly IMapper _mapper;

        public UpdateApplicationCommandHandlerSql(ILuebenContextSql LuebenContext, IMapper mapper)
        {
            _LuebenContext = LuebenContext;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateApplicationCommandSql request)
        {
            var application = await _LuebenContext.Application.FirstOrDefaultAsync(x => x.Id == request.Id).ConfigureAwait(false);
            if (application == null)
            {
                throw new EntityNotFoundException();
            }

            application = _mapper.Map(request, application);
            application.MarkUpdated();

            _LuebenContext.Application.Update(application);
            _LuebenContext.SaveChanges();

            return Unit.Value;
        }
    }
}