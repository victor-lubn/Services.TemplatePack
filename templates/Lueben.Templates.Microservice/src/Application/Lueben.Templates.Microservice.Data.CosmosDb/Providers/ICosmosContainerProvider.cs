// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.Azure.Cosmos;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Providers
{
    /// <summary>
    /// The cosmos container provider exposes a means of providing
    /// an instance to the configured <see cref="Container"/> object.
    /// </summary>
    public interface ICosmosContainerProvider<TItem>
        where TItem : Entity
    {
        /// <summary>
        /// Asynchronously gets the configured <see cref="Container"/> instance that corresponds to the
        /// cosmos <see cref="RepositoryOptions"/>.
        /// </summary>
        /// <returns>A <see cref="Container"/> instance.</returns>
        Task<Container> GetContainerAsync();
    }
}