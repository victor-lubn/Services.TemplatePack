using System.Reflection;
using Lueben.Microservice.DurableFunction;
using Lueben.Microservice.Extensions.Configuration;
using Lueben.Microservice.Mediator;
using Lueben.Microservice.Options;
using Lueben.Templates.Orchestrator.Function;
using Lueben.Templates.Orchestrator.Function.Handlers;
using Lueben.Templates.Orchestrator.Function.Validation;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Lueben.Templates.Orchestrator.Function
{
    public class Startup : FunctionsStartup
    {
        private static readonly Assembly[] HandlerAssemblies = { typeof(DurableHttpRouteEventHandler).Assembly };

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddLuebenAzureAppConfiguration()
                .RegisterConfiguration<WorkflowOptions>(nameof(WorkflowOptions))
                .AddLogging()
                .AddTelemetryLogging()
                .AddTransient<IEventValidator, EventValidator>()
                .AddMediatr(typeof(INotificationHandler<>), HandlerAssemblies)
                .AddOrchestratorFunctionHealthChecks()
                .AddHttpEventHandler();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
            builder.ConfigurationBuilder.AddLuebenAzureAppConfiguration();
        }
    }
}