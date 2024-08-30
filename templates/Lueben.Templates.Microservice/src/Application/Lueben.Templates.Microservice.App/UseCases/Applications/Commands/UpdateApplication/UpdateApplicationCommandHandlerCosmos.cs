using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication
{
    public class UpdateApplicationCommandHandlerCosmos : IRequestHandler<UpdateApplicationCommandCosmos, Unit>
    {
        private readonly IRepositoryCosmos<Application> _applicationRepository;
        private readonly IMapper _mapper;

        public UpdateApplicationCommandHandlerCosmos(IRepositoryCosmos<Application> applicationRepository, IMapper mapper)
        {
            _applicationRepository = applicationRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateApplicationCommandCosmos request)
        {
            var applicationCollection = await _applicationRepository.GetAsync(application => application.Id == request.Id);
            var application = applicationCollection.FirstOrDefault();

            if (application == null)
            {
                throw new EntityNotFoundException();
            }

            var applicationEntity = _mapper.Map(request, application);

            await _applicationRepository.UpdateAsync(applicationEntity, applicationEntity.PartitionKey);

            return Unit.Value;
        }
    }
}
