// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.Infrastructure
{
    /// <summary>
    /// A factory abstraction for a component that can
    /// create <see cref="IRepositoryCosmos{TItem}"/> instances.
    /// </summary>
    public interface IRepositoryFactoryCosmos
    {
        /// <summary>
        /// Gets an <see cref="IRepositoryCosmos{TItem}"/> instance for the
        /// given <typeparamref name="TItem"/> type.
        /// </summary>
        /// <typeparam name="TItem">The item type that corresponds to the respoitory.</typeparam>
        /// <returns>An <see cref="IRepositoryCosmos{TItem}"/> of <typeparamref name="TItem"/>.</returns>
        IRepositoryCosmos<TItem> RepositoryOf<TItem>()
            where TItem : Entity;
    }
}