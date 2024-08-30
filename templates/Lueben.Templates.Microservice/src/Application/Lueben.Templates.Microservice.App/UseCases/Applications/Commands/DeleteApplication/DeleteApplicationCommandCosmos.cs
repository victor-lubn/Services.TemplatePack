using Lueben.Microservice.EntityFunction.Models;
using Lueben.Microservice.Mediator;

namespace Lueben.Templates.Microservice.App.UseCases.Applications.Commands.DeleteApplication
{
    public class DeleteApplicationCommandCosmos : ApplicationCommandBase, IRequest<Unit>, IEntityOperation<string>
    {
    }
}