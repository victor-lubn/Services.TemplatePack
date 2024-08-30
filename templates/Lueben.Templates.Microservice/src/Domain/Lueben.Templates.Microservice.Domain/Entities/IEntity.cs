// #if (Cosmos)
using Microsoft.Azure.Cosmos;

// #endif
namespace Lueben.Templates.Microservice.Domain.Entities
{
    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the item's globally unique identifier.
        /// </summary>
        string Id { get; set; }

        // #if (Cosmos)

        /// <summary>
        /// Gets or sets the item's type name.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Gets the item's PartitionKey.
        /// </summary>
        PartitionKey PartitionKey { get; }

        // #endif
        void MarkUpdated();
    }
}
