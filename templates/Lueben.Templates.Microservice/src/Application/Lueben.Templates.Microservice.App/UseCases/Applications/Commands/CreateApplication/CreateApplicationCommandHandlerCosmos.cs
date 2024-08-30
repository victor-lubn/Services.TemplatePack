using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Commands.CreateApplication
{
    public class CreateApplicationCommandHandlerCosmos : IRequestHandler<CreateApplicationCommandCosmos, string>
    {
        private readonly IRepositoryCosmos<Application> _applicationRepository;
        private readonly IMapper _mapper;

        public CreateApplicationCommandHandlerCosmos(IRepositoryCosmos<Application> applicationRepository,
            IMapper mapper)
        {
            _applicationRepository = applicationRepository;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreateApplicationCommandCosmos request)
        {
            var appEntity = new Application();

            appEntity = _mapper.Map(request, appEntity);

            var application = await _applicationRepository.CreateAsync(appEntity);

            return application.Id;
        }
    }
}
