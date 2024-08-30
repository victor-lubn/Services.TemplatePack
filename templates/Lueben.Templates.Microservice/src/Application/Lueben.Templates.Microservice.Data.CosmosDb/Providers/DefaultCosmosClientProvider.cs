// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Lueben.Templates.Microservice.Data.CosmosDb.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Providers
{
    [ExcludeFromCodeCoverage]
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class DefaultCosmosClientProvider : ICosmosClientProvider, IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        private readonly Lazy<CosmosClient> _lazyCosmosClient;
        private readonly CosmosClientOptions _cosmosClientOptions;
        private readonly RepositoryOptions _options;

        public DefaultCosmosClientProvider(
           ICosmosClientOptionsProvider cosmosClientOptionsProvider,
           IOptions<RepositoryOptions> options) : this(cosmosClientOptionsProvider?.ClientOptions, options) =>
           _ = cosmosClientOptionsProvider
               ?? throw new ArgumentNullException(
                   nameof(cosmosClientOptionsProvider), "Cosmos Client Options Provider is required.");

        private DefaultCosmosClientProvider(
            CosmosClientOptions cosmosClientOptions,
            IOptions<RepositoryOptions> options)
        {
            _cosmosClientOptions = cosmosClientOptions
                ?? throw new ArgumentNullException(
                    nameof(cosmosClientOptions), "Cosmos Client options are required.");

            _options = options?.Value
                ?? throw new ArgumentNullException(
                    nameof(options), "Repository options are required.");

            _lazyCosmosClient = new Lazy<CosmosClient>(
                () => new CosmosClient(_options.CosmosConnectionString, _cosmosClientOptions));
        }

        public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume) =>
            consume?.Invoke(_lazyCosmosClient.Value);

#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            if (_lazyCosmosClient?.IsValueCreated ?? false)
            {
                _lazyCosmosClient?.Value?.Dispose();
            }
        }
    }
}
