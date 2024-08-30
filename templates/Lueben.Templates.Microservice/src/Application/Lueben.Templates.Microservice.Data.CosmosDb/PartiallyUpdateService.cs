using System;
using System.Reflection;
using System.Threading.Tasks;
using Lueben.Microservice.Api.ValidationFunction.Extensions;
using Lueben.Microservice.EntityFunction.Models;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.Data.CosmosDb
{
    public class PartiallyUpdateService<TEntity, TRequest> : IPartiallyUpdateServiceCosmos<TEntity, TRequest>
        where TEntity : Entity
        where TRequest : IEntityOperation<string>
    {
        public Task ApplyChanges(TEntity entity, TRequest request)
        {
            var filledProperties = request.GetFilledProperties();

            foreach (var filledProperty in filledProperties)
            {
                PropertyInfo propertyInfo = entity.GetType().GetProperty(filledProperty);
                var requestValue = request.GetType().GetProperty(filledProperty).GetValue(request);
                propertyInfo.SetValue(entity, Convert.ChangeType(requestValue, propertyInfo.PropertyType), null);
            }

            return Task.CompletedTask;
        }
    }
}
