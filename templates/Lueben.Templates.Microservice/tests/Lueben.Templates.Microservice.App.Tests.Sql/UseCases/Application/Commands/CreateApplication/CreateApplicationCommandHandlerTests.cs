using System;
using System.Linq;
using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.Tests.Sql.Helpers;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.CreateApplication;
using Lueben.Templates.Microservice.Data.Sql;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Sql.UseCases.Application.Commands.CreateApplication
{
    public class CreateApplicationCommandHandlerTests
    {
        private readonly IServiceCollection _serviceCollection;

        public CreateApplicationCommandHandlerTests()
        {
            _serviceCollection = SetupServiceProvider();
        }

        [Fact]
        public async Task GivenCreateApplicationCommand_WhenCommandIsExecuted_ThenEntityAndApplicationEventShouldBeSavedToDatabase()
        {
            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var createCommand = new CreateApplicationCommandSql
                {
                    PreferredDepotId = "1"
                };

                var mediator = provider.GetService<IMediator>();
                var applicationId = await mediator.Send<CreateApplicationCommandSql, string>(createCommand);

                var saved = provider.GetService<ILuebenContextSql>().Application.FirstOrDefault();

                Assert.NotNull(saved);
                Assert.Equal(saved.PreferredDepotId, createCommand.PreferredDepotId);
                Assert.NotEqual(Guid.Empty.ToString(), applicationId);
            }
        }

        [Fact]
        public async Task GivenCreateApplicationCommand_WhenCommandIsExecutedAndApplicationModelIsEmpty_ThenEntityAndApplicationEventShouldBeSavedToDatabase()
        {
            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var createCommand = new CreateApplicationCommandSql();

                var mediator = provider.GetService<IMediator>();
                var applicationId = await mediator.Send<CreateApplicationCommandSql, string>(createCommand);

                var saved = provider.GetService<ILuebenContextSql>().Application.FirstOrDefault();

                Assert.NotNull(saved);
                Assert.Null(saved.PreferredDepotId);
                Assert.Null(saved.RejectionReason);
                Assert.Null(saved.Comments);

                Assert.NotEqual(Guid.Empty.ToString(), applicationId);
            }
        }

        private IServiceCollection SetupServiceProvider()
        {
            var context = MockContext();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly)
                .AddSingleton<ILuebenContextSql, LuebenContext>(x => context)
                .AddTransient<IMediator, Mediator>()
                .AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider))
                .AddTransient<IRequestHandler<CreateApplicationCommandSql, string>, CreateApplicationCommandHandlerSql>();

            return serviceCollection;
        }

        private static LuebenContext MockContext()
        {
            var context = LuebenDbContextFactory.CreateInMemory();

            return context;
        }
    }
}