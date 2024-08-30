using System.Threading.Tasks;
using Lueben.Microservice.EntityFunction.Models;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.Infrastructure
{
    public interface IPartiallyUpdateServiceCosmos<TEntity, TRequest>
        where TEntity : Entity
        where TRequest : IEntityOperation<string>
    {
        Task ApplyChanges(TEntity entity, TRequest request);
    }
}
