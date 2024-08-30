using Lueben.Microservice.DurableFunction;
using Lueben.Microservice.Extensions.Configuration;
using Lueben.Microservice.Options;
using Lueben.Templates.DurableMicroservice.Function;
using Lueben.Templates.DurableMicroservice.Function.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Lueben.Templates.DurableMicroservice.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddLuebenAzureAppConfigurationWithNoRefresher()
                .RegisterConfiguration<WorkflowOptions>(nameof(WorkflowOptions))
                .AddCircuitBreaker()
                .AddTelemetryLogging()
                .AddDependencyOneClient()
                .AddDependencyTwoClient()
                .AddDurableFunctionHealthChecks();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
            builder.ConfigurationBuilder.AddLuebenAzureAppConfiguration();
        }
    }
}