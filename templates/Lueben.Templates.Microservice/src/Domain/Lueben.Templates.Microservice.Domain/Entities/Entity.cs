using System;
// #if (Cosmos)
using Microsoft.Azure.Cosmos;
// #endif
using Newtonsoft.Json;

namespace Lueben.Templates.Microservice.Domain.Entities
{
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// Gets or sets the item's globally unique identifier.
        /// </summary>
        /// <remarks>
        /// Initialized by <see cref="Guid.NewGuid"/>.
        /// </remarks>
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // #if (Cosmos)

        /// <summary>
        /// Gets or sets the item's type name.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets the PartitionKey based on <see cref="GetPartitionKeyValue"/>.
        /// Implemented explicitly to keep out of Item API.
        /// </summary>
        public PartitionKey PartitionKey => new PartitionKey(GetPartitionKeyValue());

        /// <summary>
        /// Default constructor, assigns type name to <see cref="Type"/> property.
        /// </summary>
        public Entity()
        {
            Type = GetType().Name;
        }

        /// <summary>
        /// Gets the partition key value for the given <see cref="Item"/> type.
        /// When overridden, be sure that the <see cref="PartitionKeyPathAttribute.Path"/> value corresponds
        /// to the <see cref="JsonPropertyAttribute.PropertyName"/> value, i.e.; "/partition" and "partition"
        /// respectively. If these two values do not correspond an error will occur.
        /// </summary>
        /// <returns>The <see cref="Item.Id"/> unless overridden by the subclass.</returns>
        protected virtual string GetPartitionKeyValue() => Id;

        // #endif
        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        [JsonProperty("updated")]
        public DateTime? Updated { get; set; }

        public void MarkUpdated()
        {
            Updated = DateTime.UtcNow;
        }
    }
}
