using FluentValidation;
using Lueben.Microservice.Api.PipelineFunction;
using Lueben.Microservice.Extensions.Configuration;
using Lueben.Microservice.FileLogging;
using Lueben.Microservice.Mediator;
using Lueben.Microservice.Options;
using Lueben.Microservice.Serialization;
using Lueben.Templates.Microservice.Api;
using Lueben.Templates.Microservice.Api.Extensions;
using Lueben.Templates.Microservice.Api.Filters;
using Lueben.Templates.Microservice.Api.Validators;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.Options;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication;
// #if (Cosmos)
using Lueben.Templates.Microservice.Data.CosmosDb;
using Lueben.Templates.Microservice.Data.CosmosDb.Extensions;
// #if (Sql)
using Lueben.Templates.Microservice.Data.Sql;
// #endif
using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
// #endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Lueben.Templates.Microservice.Api
{
    public class Startup : PipelineFunctionsStartup
    {
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            ConfigureNamingStrategy();

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();

            return services
                .AddLuebenAzureAppConfigurationWithNoRefresher()
                .RegisterConfiguration<ExampleOptions>(nameof(ExampleOptions))
                .AddMediatr(typeof(IRequestHandler<,>))
                // #if (Cosmos)
                .AddCosmosRepository()
                .AddTransient<IPartiallyUpdateServiceCosmos<Application, PartiallyUpdateApplicationCommandCosmos>, PartiallyUpdateService<Application, PartiallyUpdateApplicationCommandCosmos>>()
                // #endif
                // #if (Sql)
                .AddTransient<ILuebenContextSql, LuebenContext>()
                .AddDbContext<LuebenContext>(options => ConfigureSqlServer(options, configuration))
                .AddTransient<IEntityFrameworkPatchSql, EntityFrameworkPatchSql>()
                // #endif
                .AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly, typeof(App.AutoMapperProfile.AutoMapperProfile).Assembly)
                .AddFluentValidators()
                .AddCustomOpenApi()
                .AddTelemetryLogging()
                .AddFileLogging();
        }

        public override IServiceCollection ConfigurePipeline(IServiceCollection services)
        {
#pragma warning disable 618
            return services
                .AddTransient<IFunctionFilter, RequiredHeadersFilter>()
                .AddTransient<IFunctionFilter, ModelValidationExceptionFilter>()
                .AddTransient<IFunctionFilter, EntityExceptionFilter>()
                .AddTransient<IFunctionFilter, DefaultExceptionFilter>();
#pragma warning restore 618
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
            builder.ConfigurationBuilder.AddLuebenAzureAppConfiguration();
        }

        private static void ConfigureNamingStrategy()
        {
            ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression) =>
            {
                if (memberInfo == null)
                {
                    return null;
                }

                var namingStrategy = FunctionJsonSerializerSettingsProvider.CreateNamingStrategy();
                var propName = namingStrategy.GetPropertyName(memberInfo.Name, false);
                return propName;
            };
        }

        // #if (Sql)
        private static void ConfigureSqlServer(DbContextOptionsBuilder options, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LuebenOnlineApplicationDbContext");
            options.UseSqlServer(connectionString);

            var isDevelopmentMode = configuration.GetValue("IsDevelopmentMode", false);
            if (isDevelopmentMode)
            {
                options.EnableSensitiveDataLogging();
            }

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        // #endif
    }
}