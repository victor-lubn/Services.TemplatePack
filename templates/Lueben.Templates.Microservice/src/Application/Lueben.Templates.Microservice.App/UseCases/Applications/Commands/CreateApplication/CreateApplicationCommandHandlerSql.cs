using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.Options;
using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Commands.CreateApplication
{
    public class CreateApplicationCommandHandlerSql : IRequestHandler<CreateApplicationCommandSql, string>
    {
        private readonly ILuebenContextSql _LuebenContext;
        private readonly IMapper _mapper;
        private readonly IOptionsSnapshot<ExampleOptions> _options;

        public CreateApplicationCommandHandlerSql(ILuebenContextSql LuebenContext,
            IMapper mapper,
            IOptionsSnapshot<ExampleOptions> options)
        {
            _LuebenContext = LuebenContext;
            _mapper = mapper;
            _options = options;
        }

        public async Task<string> Handle(CreateApplicationCommandSql request)
        {
            var appEntity = new Application();

            // Example of reading a secret that can be stored as a key vault references in azure app configuration.
            var secret = _options.Value.Secret;

            appEntity = _mapper.Map(request, appEntity);

            await _LuebenContext.Application.AddAsync(appEntity);
            await _LuebenContext.SaveChangesAsync();

            return appEntity.Id;
        }
    }
}