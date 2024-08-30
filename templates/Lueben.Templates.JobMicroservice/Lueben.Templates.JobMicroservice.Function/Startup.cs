using Lueben.Microservice.DurableFunction;
using Lueben.Microservice.Extensions.Configuration;
using Lueben.Templates.JobMicroservice.Function;
using Lueben.Templates.JobMicroservice.Function.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Lueben.Templates.JobMicroservice.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .RegisterConfiguration<JobOptions>(nameof(JobOptions))
                .RegisterConfiguration<WorkflowOptions>(nameof(WorkflowOptions))
                .AddEventDataSender()
                .AddTelemetryLogging()
                .AddDependencyOneClient()
                .AddJobFunctionHealthChecks();
        }
    }
}