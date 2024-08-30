// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Lueben.Templates.Microservice.Data.CosmosDb
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public class DefaultRepositoryFactory : IRepositoryFactoryCosmos
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor for the default repository factory.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultRepositoryFactory(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(
                    nameof(serviceProvider),
                    "A service provider instance is required.");

        /// <inheritdoc/>
        public IRepositoryCosmos<TItem> RepositoryOf<TItem>()
            where TItem : Entity =>
            _serviceProvider.GetRequiredService<IRepositoryCosmos<TItem>>();
    }
}
