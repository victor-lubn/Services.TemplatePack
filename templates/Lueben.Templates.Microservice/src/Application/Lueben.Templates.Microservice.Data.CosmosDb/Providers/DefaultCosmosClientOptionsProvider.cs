using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using Lueben.Templates.Microservice.Data.CosmosDb.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Providers
{
    [ExcludeFromCodeCoverage]
    public class DefaultCosmosClientOptionsProvider : ICosmosClientOptionsProvider
    {
        private readonly Lazy<CosmosClientOptions> _lazyClientOptions;

        public CosmosClientOptions ClientOptions => _lazyClientOptions.Value;

        /// <summary>
        /// Default <see cref="ICosmosClientOptionsProvider"/> implementation.
        /// </summary>
        /// <param name="serviceProvider">Service provider implementation.</param>
        /// <param name="configuration">Service configuration implementation.</param>
        public DefaultCosmosClientOptionsProvider(
            IServiceProvider serviceProvider,
            IConfiguration configuration) =>
            _lazyClientOptions = new Lazy<CosmosClientOptions>(
                () => CreateCosmosClientOptions(serviceProvider, configuration));

        private CosmosClientOptions CreateCosmosClientOptions(
            IServiceProvider serviceProvider,
            IConfiguration configuration) =>
            new CosmosClientOptions
            {
                HttpClientFactory = () => ClientFactory(serviceProvider),
                AllowBulkExecution =
                    configuration.GetSection(nameof(RepositoryOptions))
                        .GetValue<bool>(nameof(RepositoryOptions.AllowBulkExecution))
            };

        private HttpClient ClientFactory(IServiceProvider serviceProvider)
        {
            HttpClient client =
                serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();

            string version =
                Assembly.GetExecutingAssembly()
                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                        .InformationalVersion;

            client.DefaultRequestHeaders
                  .UserAgent
                  .ParseAdd($"ievangelist-cosmos-repository-sdk/{version}");

            return client;
        }
    }
}