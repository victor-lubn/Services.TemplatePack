using System.Collections.Generic;
using System.Reflection;
using Azure.Identity;
using Lueben.ApplicationInsights.Headers;
using Lueben.Microservice.Api.PipelineFunction.Constants;
using Lueben.Microservice.ApplicationInsights.Extensions;
using Lueben.Microservice.ApplicationInsights.TelemetryInitializers;
using Lueben.Microservice.OpenApi.Extensions;
using Lueben.Microservice.OpenApi.Version.Extensions;
using Lueben.Microservice.OpenApi.Visitors;
using Lueben.Microservice.Options;
using Lueben.Templates.Microservice.Functions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lueben.Templates.Microservice.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomOpenApi(this IServiceCollection services)
        {
            return services.AddOpenApiVersion()
                .AddOpenApi((serviceProvider, options) =>
                {
                    options.AddCommonHeader(Headers.SourceConsumer, "template consumer application name", true)
                        .AddCommonHeader(Headers.IdentityReference, "template consumer application name", true)
                        .AddVisitor<AuditDateTimeFieldsVisitor>()
                        .AddVisitor<ComplexObjectVisitor>()
                        .AddVisitor<Int32StringEnumTypeVisitor>()
                        .AddVisitor<NullableEnumVisitor>()
                        .AddVisitor<ReadonlyAttributeVisitor>()
                        .AddVisitor<ShortDateVisitor>()
                        .AddVisitor(vc => new FluentValidationVisitor(vc, serviceProvider));
                });
        }

        public static IConfigurationBuilder AddLuebenAzureAppConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            var token = new ChainedTokenCredential(new ManagedIdentityCredential(), new VisualStudioCredential());
            var appPrefix = Assembly.GetCallingAssembly().GetName().Name;
            var globalPrefix = Helpers.GetDefaultGlobalPrefix();
            return configurationBuilder.AddLuebenAzureAppConfiguration(token, appPrefix, globalPrefix);
        }

        public static IServiceCollection AddTelemetryLogging(this IServiceCollection services)
        {
            var properties = LogConstants.AllProperties;
            var headers = new List<string>
            {
                Headers.IdentityReference,
                Headers.SourceConsumer,
                "Referer"
            };

            return services.AddLogging(options => options.AddFilter("Lueben", LogLevel.Information))
                .AddApplicationInsightsTelemetry(options =>
                {
                    options.AddTelemetryInitializer(_ => new CustomDataPropertyTelemetryInitializer(properties))
                        .AddTelemetryInitializer(sp => new HeadersTelemetryInitializer(sp.GetService<IHttpContextAccessor>(), headers));
                });
        }
    }
}