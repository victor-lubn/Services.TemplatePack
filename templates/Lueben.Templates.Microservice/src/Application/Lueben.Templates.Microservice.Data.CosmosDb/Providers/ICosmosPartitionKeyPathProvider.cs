// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Providers
{
    /// <summary>
    /// The cosmos partition key path provider exposes the ability
    /// to get an <see cref="IItem"/>'s partition key path.
    /// </summary>
    public interface ICosmosPartitionKeyPathProvider
    {
        /// <summary>
        /// Gets the partition key path for a given <typeparamref name="TItem"/> type.
        /// </summary>
        /// <typeparam name="TItem">The item for which the partition key path corresponds.</typeparam>
        /// <returns>A string value representing the partition key path, i.e.; "/partion".</returns>
        string GetPartitionKeyPath<TItem>()
            where TItem : Entity;
    }
}
