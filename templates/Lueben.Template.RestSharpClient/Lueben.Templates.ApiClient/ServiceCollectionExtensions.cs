using System.Diagnostics.CodeAnalysis;
using Lueben.Microservice.Extensions.Configuration;
using Lueben.Microservice.RestSharpClient.Authentication;
using Lueben.Microservice.RestSharpClient.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace Lueben.Templates.ApiClient
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRestSharpClientFactory(this IServiceCollection services)
        {
            return services
                .AddMicroserviceRestSharpClientFactory()
                .RegisterConfiguration<ConfidentialClientApplicationOptions>("AzureAD")
                .AddTransient<IServiceApiAuthorizer, AadConfidentialClient>();
        }

        public static IServiceCollection AddApiClient(this IServiceCollection services)
        {
            return services
                .RegisterConfiguration<ApiOptions>(nameof(ApiOptions))
                .AddTransient<IApiClient, ApiClient>();
        }
    }
}
