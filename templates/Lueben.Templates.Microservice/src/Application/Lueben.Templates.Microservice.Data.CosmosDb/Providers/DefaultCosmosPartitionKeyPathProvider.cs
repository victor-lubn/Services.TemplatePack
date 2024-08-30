// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Lueben.Templates.Microservice.Domain.Attributes;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Providers
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class DefaultCosmosPartitionKeyPathProvider
        : ICosmosPartitionKeyPathProvider
    {
        private static readonly Type _partitionKeyNameAttributeType = typeof(PartitionKeyPathAttributeCosmos);
        private static readonly ConcurrentDictionary<Type, string> _partionKeyNameMap =
            new ConcurrentDictionary<Type, string>();

        /// <inheritdoc />
        public string GetPartitionKeyPath<TItem>()
            where TItem : Entity
            =>
            _partionKeyNameMap.GetOrAdd(typeof(TItem), GetPartitionKeyNameFactory);

        private static string GetPartitionKeyNameFactory(Type type)
        {
            PartitionKeyPathAttributeCosmos attribute =
                Attribute.GetCustomAttribute(type, _partitionKeyNameAttributeType)
                as PartitionKeyPathAttributeCosmos;

            return attribute?.Path ?? "/id";
        }
    }
}
