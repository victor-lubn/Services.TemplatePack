// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Data.CosmosDb.Extensions;
using Lueben.Templates.Microservice.Data.CosmosDb.Options;
using Lueben.Templates.Microservice.Data.CosmosDb.Providers;
using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Lueben.Templates.Microservice.Data.CosmosDb
{
    [ExcludeFromCodeCoverage]
    internal class DefaultRepository<TItem> : IRepositoryCosmos<TItem>
        where TItem : Entity
    {
        private readonly ICosmosContainerProvider<TItem> _containerProvider;
        private readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;
        private readonly ILogger<DefaultRepository<TItem>> _logger;

        private (bool OptimizeBandwidth, ItemRequestOptions Options) RequestOptions =>
            (_optionsMonitor.CurrentValue.OptimizeBandwidth, new ItemRequestOptions
            {
                EnableContentResponseOnWrite = !_optionsMonitor.CurrentValue.OptimizeBandwidth
            });

        public DefaultRepository(
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICosmosContainerProvider<TItem> containerProvider,
            ILogger<DefaultRepository<TItem>> logger) =>
            (_optionsMonitor, _containerProvider, _logger) = (optionsMonitor, containerProvider, logger);

        public ValueTask<TItem> GetAsync(
            string id,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default) =>
            GetAsync(id, new PartitionKey(partitionKeyValue ?? id));

        public async ValueTask<TItem> GetAsync(
            string id,
            PartitionKey partitionKey,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            ItemResponse<TItem> response =
                await container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            TItem item = response.Resource;

            TryLogDebugDetails(_logger, () => $"Read: {JsonConvert.SerializeObject(item)}");

            return string.IsNullOrEmpty(item.Type) || item.Type == typeof(TItem).Name ? item : default;
        }

        public async ValueTask<IEnumerable<TItem>> GetAsync(
            Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            IQueryable<TItem> query =
                container.GetItemLinqQueryable<TItem>()
                    .Where(predicate.Compose(
                        item => !CosmosLinqExtensions.IsDefined(item.Type) || item.Type == typeof(TItem).Name, Expression.AndAlso));

            TryLogDebugDetails(_logger, () => $"Read: {query}");

            using (FeedIterator<TItem> iterator = query.ToFeedIterator())
            {
                List<TItem> results = new List<TItem>();
                while (iterator.HasMoreResults)
                {
                    foreach (TItem result in await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false))
                    {
                        results.Add(result);
                    }
                }

                return results;
            }
        }

        public async ValueTask<IEnumerable<TItem>> GetByQueryAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Read {query}");

            QueryDefinition queryDefinition = new QueryDefinition(query);
            return await IterateQueryInternalAsync(container, queryDefinition, cancellationToken);
        }

        public async ValueTask<IEnumerable<TItem>> GetByQueryAsync(
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Read {queryDefinition.QueryText}");

            return await IterateQueryInternalAsync(container, queryDefinition, cancellationToken);
        }

        public async ValueTask<TItem> CreateAsync(
            TItem value,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            value.Created = DateTime.UtcNow;

            ItemResponse<TItem> response =
                await container.CreateItemAsync(value, value.PartitionKey, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Created: {JsonConvert.SerializeObject(value)}");

            return response.Resource;
        }

        public async ValueTask<IEnumerable<TItem>> CreateAsync(
            IEnumerable<TItem> values,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Task<TItem>> creationTasks =
                values.Select(value => CreateAsync(value, cancellationToken).AsTask())
                    .ToList();

            _ = await Task.WhenAll(creationTasks).ConfigureAwait(false);

            return creationTasks.Select(x => x.Result);
        }

        public async Task<TItem> UpdateAsync(TItem value, PartitionKey partitionKey)
        {
            (bool optimizeBandwidth, ItemRequestOptions options) = RequestOptions;
            Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            value.Updated = DateTime.UtcNow;

            ItemResponse<TItem> response =
                await container.UpsertItemAsync(value, partitionKey, options).ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Updated: {JsonConvert.SerializeObject(value)}");

            return optimizeBandwidth ? value : response.Resource;
        }

        public async Task<IEnumerable<TItem>> UpdateAsync(IEnumerable<TItem> values)
        {
            await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            IEnumerable<Task<TItem>> creationTasks =
                values.Select(async v => await UpdateAsync(v, v.PartitionKey).ConfigureAwait(false)).ToList();

            _ = await Task.WhenAll(creationTasks).ConfigureAwait(false);

            return creationTasks.Select(x => x.Result);
        }

        public ValueTask DeleteAsync(
            TItem value,
            CancellationToken cancellationToken = default) =>
            DeleteAsync(value.Id, value.PartitionKey, cancellationToken);

        public ValueTask DeleteAsync(
            string id,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default) =>
            DeleteAsync(id, new PartitionKey(partitionKeyValue ?? id));

        public async ValueTask DeleteAsync(
            string id,
            PartitionKey partitionKey,
            CancellationToken cancellationToken = default)
        {
            ItemRequestOptions options = RequestOptions.Options;
            Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            _ = await container.DeleteItemAsync<TItem>(id, partitionKey, options, cancellationToken)
                .ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Deleted: {id}");
        }

        private static async Task<IEnumerable<TItem>> IterateQueryInternalAsync(
            Container container,
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken)
        {
            using (FeedIterator<TItem> queryIterator = container.GetItemQueryIterator<TItem>(queryDefinition))
            {
                List<TItem> results = new List<TItem>();
                while (queryIterator.HasMoreResults)
                {
                    FeedResponse<TItem> response = await queryIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
                    results.AddRange(response.Resource);
                }

                return results;
            }
        }

        private static void TryLogDebugDetails(ILogger logger, Func<string> getMessage)
        {
            if (logger?.IsEnabled(LogLevel.Debug) ?? false)
            {
                logger.LogDebug(getMessage());
            }
        }
    }
}