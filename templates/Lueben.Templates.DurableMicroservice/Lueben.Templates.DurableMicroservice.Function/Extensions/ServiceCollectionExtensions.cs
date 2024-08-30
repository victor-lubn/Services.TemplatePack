using System.Reflection;
using Azure.Identity;
using Castle.DynamicProxy;
using Lueben.Microservice.ApplicationInsights.Extensions;
using Lueben.Microservice.ApplicationInsights.TelemetryInitializers;
using Lueben.Microservice.CircuitBreaker;
using Lueben.Microservice.DurableFunction.Extensions;
using Lueben.Microservice.DurableFunction.HealthCheck;
using Lueben.Microservice.Extensions.Configuration;
using Lueben.Microservice.HealthChecks;
using Lueben.Microservice.Interceptors;
using Lueben.Microservice.Options;
using Lueben.Templates.DurableMicroservice.Function.Constants;
using Lueben.Templates.Eventing.Clients.Stub;
using Lueben.Templates.Eventing.Clients.Stub.Stubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Registry;

namespace Lueben.Templates.DurableMicroservice.Function.Extensions
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
            return services.AddApplicationInsightsTelemetry(options =>
            {
                options.AddTelemetryInitializer(_ => new CustomDataPropertyTelemetryInitializer(LogConstants.AllProperties));
            });
        }

        public static IServiceCollection AddCircuitBreaker(this IServiceCollection services)
        {
            return services
                .RegisterNamedOptions<CircuitBreakerSettings>()
                .AddMemoryCache()
                .AddSingleton(typeof(OptionsManager<CircuitBreakerSettings>))
                .AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>()
                .AddSingleton<IPolicyRegistry<string>>(new PolicyRegistry())
                .AddSingleton<IDurableCircuitBreakerClient, DurableCircuitBreakerClient>()
                .AddSingleton<ICircuitBreakerClient, CircuitBreakerClient>()
                .AddDurableFunctions()
                .AddSingleton<ICircuitBreakerStateChecker, CircuitBreakerStateChecker>();
        }

        public static IServiceCollection AddDurableFunctionHealthChecks(this IServiceCollection services)
        {
            services
                .RegisterConfiguration<WorkflowHealthCheckOptions>(nameof(WorkflowHealthCheckOptions))
                .AddFunctionHealthChecks()
                .AddWorkflowHealthCheck();

            return services;
        }

        /// <summary>
        /// Example of adding dependency with circuit breaker.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDependencyOneClient(this IServiceCollection services)
        {
            return services.AddTransient(provider =>
            {
                IDependencyOneClient target = new MicroserviceDependencyClientStub();
                var circuitBreakerInterceptor = new CircuitBreakerInterceptor(provider.GetService<ICircuitBreakerClient>(), CircuitBreakerInstanceIdsConstants.DependencyOneInstanceId);
                var generator = new ProxyGenerator();
                return generator.CreateInterfaceProxyWithTarget(target, circuitBreakerInterceptor);
            });
        }

        /// <summary>
        /// Example of adding dependency with circuit breaker.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDependencyTwoClient(this IServiceCollection services)
        {
            return services.AddTransient(provider =>
            {
                IDependencyTwoClient target = new MicroserviceDependencyClientStub();
                var circuitBreakerInterceptor = new CircuitBreakerInterceptor(provider.GetService<ICircuitBreakerClient>(), CircuitBreakerInstanceIdsConstants.DependencyTwoInstanceId);
                var generator = new ProxyGenerator();
                return generator.CreateInterfaceProxyWithTarget(target, circuitBreakerInterceptor);
            });
        }
    }
}