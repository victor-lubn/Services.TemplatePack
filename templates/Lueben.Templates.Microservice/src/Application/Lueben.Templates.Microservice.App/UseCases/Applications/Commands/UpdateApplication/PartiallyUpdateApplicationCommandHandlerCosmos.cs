using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication
{
    public class PartiallyUpdateApplicationCommandHandlerCosmos : IRequestHandler<PartiallyUpdateApplicationCommandCosmos, Unit>
    {
        private readonly IRepositoryCosmos<Application> _applicationRepository;
        private readonly IMapper _mapper;
        private readonly IPartiallyUpdateServiceCosmos<Application, PartiallyUpdateApplicationCommandCosmos> _partiallyUpdateService;

        public PartiallyUpdateApplicationCommandHandlerCosmos(IRepositoryCosmos<Application> applicationRepository,
            IMapper mapper,
            IPartiallyUpdateServiceCosmos<Application, PartiallyUpdateApplicationCommandCosmos> partiallyUpdateService)
        {
            _applicationRepository = applicationRepository;
            _mapper = mapper;
            _partiallyUpdateService = partiallyUpdateService;
        }

        public async Task<Unit> Handle(PartiallyUpdateApplicationCommandCosmos request)
        {
            var applicationCollection = await _applicationRepository.GetAsync(application => application.Id == request.Id);
            var application = applicationCollection.FirstOrDefault();

            if (application == null)
            {
                throw new EntityNotFoundException();
            }

            await _partiallyUpdateService.ApplyChanges(application, request);

            var appEntity = new Application();

            appEntity = _mapper.Map(application, appEntity);

            await _applicationRepository.UpdateAsync(appEntity, appEntity.PartitionKey);

            return Unit.Value;
        }
    }
}