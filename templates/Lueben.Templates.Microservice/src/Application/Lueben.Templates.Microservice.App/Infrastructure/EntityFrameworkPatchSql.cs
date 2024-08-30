using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Api.ValidationFunction.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lueben.Templates.Microservice.App.Infrastructure
{
    public class EntityFrameworkPatchSql : IEntityFrameworkPatchSql
    {
        private readonly ILuebenContextSql _LuebenContext;
        private readonly IMapper _mapper;

        public EntityFrameworkPatchSql(ILuebenContextSql LuebenContext, IMapper mapper)
        {
            _LuebenContext = LuebenContext;
            _mapper = mapper;
        }

        public async Task ApplyPatch<TEntity, TDto>(TDto dto)
            where TEntity : class
        {
            if (dto == null)
            {
                throw new ArgumentNullException($"{nameof(dto)}", message: $"{nameof(dto)} cannot be null.");
            }

            var entityToUpdate = _mapper.Map<TEntity>(dto);
            var propertiesToUpdate = GetEntityPropertiesToUpdate(entityToUpdate).ToList();

            _LuebenContext.Attach(entityToUpdate);

            foreach (var property in propertiesToUpdate)
            {
                var propertyToUpdate = _LuebenContext.Entry(entityToUpdate).Property(property);
                propertyToUpdate.IsModified = true;
            }

            await _LuebenContext.SaveChangesAsync();
        }

        private IList<string> GetEntityPropertiesToUpdate<TEntity>(TEntity entityToUpdate)
            where TEntity : class
        {
            var entityAllProperties = _LuebenContext.Entry(entityToUpdate).Properties
                .Where(x => !x.Metadata.IsKey())
                .Select(x => x.Metadata.Name).ToList();
            var entityProperties = entityToUpdate.GetFilledProperties().ToList();

            return entityProperties.Where(x => entityAllProperties.Contains(x)).ToList();
        }
    }
}
