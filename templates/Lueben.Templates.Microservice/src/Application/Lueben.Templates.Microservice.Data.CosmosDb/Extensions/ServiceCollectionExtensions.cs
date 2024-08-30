using System;
using System.Diagnostics.CodeAnalysis;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Data.CosmosDb.Options;
using Lueben.Templates.Microservice.Data.CosmosDb.Programmability;
using Lueben.Templates.Microservice.Data.CosmosDb.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Extensions
{
    /// <summary>
    /// Extension methods for adding and configuring the Azure Cosmos DB services.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the services required to consume any number of <see cref="IRepositoryCosmos{TItem}"/>
        /// instances to interact with Cosmos DB.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="setupAction">An action to configure the repository options.</param>
        /// <returns>The same service collection that was provided, with the required cosmos services.</returns>
        public static IServiceCollection AddCosmosRepository(
            this IServiceCollection services,
            Action<RepositoryOptions> setupAction = default)
        {
            if (services is null)
            {
                throw new ArgumentNullException(
                    nameof(services), "A service collection is required.");
            }

            services.AddOptions<RepositoryOptions>()
                    .Configure<IConfiguration>(
                (settings, configuration) =>
                    configuration.GetSection(nameof(RepositoryOptions)).Bind(settings));

            services.AddLogging()
                    .AddHttpClient()
                    .AddSingleton<ICosmosClientOptionsProvider, DefaultCosmosClientOptionsProvider>()
                    .AddSingleton<ICosmosClientProvider, DefaultCosmosClientProvider>()
                    .AddCosmosProgrammability()
                    .AddSingleton(typeof(ICosmosContainerProvider<>), typeof(DefaultCosmosContainerProvider<>))
                    .AddSingleton<ICosmosPartitionKeyPathProvider, DefaultCosmosPartitionKeyPathProvider>()
                    .AddSingleton(typeof(IRepositoryCosmos<>), typeof(DefaultRepository<>))
                    .AddSingleton<IRepositoryFactoryCosmos, DefaultRepositoryFactory>();

            if (setupAction != default)
            {
                services.PostConfigure(setupAction);
            }

            return services;
        }

        private static IServiceCollection AddCosmosProgrammability(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), "A service collection is required.");
            }

            services
                .AddSingleton<ICosmosProgrammabilityFactory, CosmosProgrammabilityFactory>()
                .AddSingleton<ICosmosProgrammabilityObject, CosmosStoredProcedures>();

            return services;
        }
    }
}
