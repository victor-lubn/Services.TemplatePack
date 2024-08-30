using System.Reflection;
using Azure.Identity;
using Lueben.ApplicationInsights;
using Lueben.Microservice.ApplicationInsights;
using Lueben.Microservice.DurableFunction.HealthCheck;
using Lueben.Microservice.DurableHttpRouteFunction;
using Lueben.Microservice.EventHub;
using Lueben.Microservice.EventHub.HealthCheck;
using Lueben.Microservice.Extensions.Configuration;
using Lueben.Microservice.HealthChecks;
using Lueben.Microservice.Options;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lueben.Templates.Orchestrator.Function
{
    public static class ServiceCollectionExtensions
    {
        public static IConfigurationBuilder AddLuebenAzureAppConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            var token = new ChainedTokenCredential(new ManagedIdentityCredential(), new VisualStudioCredential());
            var appPrefix = Assembly.GetCallingAssembly().GetName().Name;
            var globalPrefix = Helpers.GetDefaultGlobalPrefix();
            return configurationBuilder.AddLuebenAzureAppConfiguration(token, appPrefix, globalPrefix);
        }

        public static IServiceCollection AddTelemetryLogging(this IServiceCollection services)
        {
            return services
                .RegisterConfiguration<ApplicationLogOptions>(nameof(ApplicationLogOptions))
                .AddLogging()
                .AddSingleton<ITelemetryInitializer, ApplicationTelemetryInitializer>()
                .AddSingleton<ILoggerService, ApplicationInsightLoggerService>();
        }

        public static IServiceCollection AddOrchestratorFunctionHealthChecks(this IServiceCollection services)
        {
            return services
                .AddTransient<IEventHubHealthCheckService, EventHubHealthCheckService>()
                .RegisterConfiguration<EventHubOptions>(nameof(EventHubOptions))
                .RegisterConfiguration<WorkflowHealthCheckOptions>(nameof(WorkflowHealthCheckOptions))
                .AddFunctionHealthChecks()
                .AddEventHubHealthCheck()
                .AddWorkflowHealthCheck()
                .Services;
        }

        public static IServiceCollection AddHttpEventHandler(this IServiceCollection services)
        {
            return services.RegisterNamedOptions<HttpEventHandlerOptions>();
        }
    }
}