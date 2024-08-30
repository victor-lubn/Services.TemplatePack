using System.Linq;
using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Commands.DeleteApplication
{
    public class DeleteApplicationCommandHandlerCosmos : IRequestHandler<DeleteApplicationCommandCosmos, Unit>
    {
        private readonly IRepositoryCosmos<Application> _applicationRepository;

        public DeleteApplicationCommandHandlerCosmos(IRepositoryCosmos<Application> applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<Unit> Handle(DeleteApplicationCommandCosmos request)
        {
            var applicationCollection = await _applicationRepository.GetAsync(application => application.Id == request.Id);
            var application = applicationCollection.FirstOrDefault();

            if (application == null)
            {
                throw new EntityNotFoundException();
            }

            await _applicationRepository.DeleteAsync(application.Id);

            return Unit.Value;
        }
    }
}
