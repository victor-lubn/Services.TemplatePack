// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Providers
{
    /// <summary>
    /// The cosmos client provider exposes a means of providing
    /// an instance to the configured <see cref="CosmosClient"/> object,
    /// which is shared.
    /// </summary>
    public interface ICosmosClientProvider
    {
        Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume);
    }
}
