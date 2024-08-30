using Lueben.ApplicationInsights;
using Lueben.Microservice.ApplicationInsights.TelemetryInitializers;
using Lueben.Microservice.DurableFunction.HealthCheck;
using Lueben.Microservice.EventHub;
using Lueben.Microservice.Extensions.Configuration;
using Lueben.Microservice.HealthChecks;
using Lueben.Templates.Eventing.Clients.Stub;
using Lueben.Templates.Eventing.Clients.Stub.Stubs;
using Lueben.Templates.JobMicroservice.Function.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Lueben.Templates.JobMicroservice.Function.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTelemetryLogging(this IServiceCollection services)
        {
            services.RegisterConfiguration<ApplicationLogOptions>(nameof(ApplicationLogOptions));
            services.AddLogging();

            return services
                .AddSingleton<ITelemetryInitializer, ApplicationTelemetryInitializer>()
                .AddSingleton<ITelemetryInitializer>(p => new CustomDataPropertyTelemetryInitializer(LogConstants.AllProperties));
        }

        public static IServiceCollection AddDependencyOneClient(this IServiceCollection services)
        {
            return services.AddTransient<IDependencyOneClient, JobDependencyClientStub>();
        }

        public static IServiceCollection AddEventDataSender(this IServiceCollection services)
        {
            return services.AddTransient<IEventDataSender, EventDataSenderStub>();
        }

        public static IServiceCollection AddJobFunctionHealthChecks(this IServiceCollection services)
        {
            services
                .RegisterConfiguration<WorkflowHealthCheckOptions>(nameof(WorkflowHealthCheckOptions))
                .AddFunctionHealthChecks()
                .AddWorkflowHealthCheck(JobMicroserviceTriggerFunction.InstanceId);

            return services;
        }
    }
}