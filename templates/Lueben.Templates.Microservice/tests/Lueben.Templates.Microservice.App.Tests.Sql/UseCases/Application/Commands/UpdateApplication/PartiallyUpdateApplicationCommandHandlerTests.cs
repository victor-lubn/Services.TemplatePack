using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.Tests.Sql.Helpers;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication;
using Lueben.Templates.Microservice.Data.Sql;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Sql.UseCases.Application.Commands.UpdateApplication
{
    public class PartiallyUpdateApplicationCommandHandlerTests
    {
        private readonly string _applicationId = Guid.NewGuid().ToString();

        private readonly IServiceCollection _serviceCollection;

        public PartiallyUpdateApplicationCommandHandlerTests()
        {
            _serviceCollection = SetupServiceProvider();
        }

        [Fact]
        public async Task GivenPartiallyUpdateApplicationCommand_WhenCommandIsExecuted_ThenEntityShouldBeUpdatedInTheDatabase()
        {
            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var mockUpdateCommand = new PartiallyUpdateApplicationCommandSql
                {
                    Id = _applicationId,
                    PreferredDepotId = "2"
                };

                var mediator = provider.GetService<IMediator>();
                await mediator.Send<PartiallyUpdateApplicationCommandSql, Unit>(mockUpdateCommand);

                var updated = provider.GetService<ILuebenContextSql>().Application.FirstOrDefault();

                Assert.NotNull(updated);
                Assert.Equal(updated.PreferredDepotId, mockUpdateCommand.PreferredDepotId);
            }
        }

        [Fact]
        public async Task GivenPartiallyUpdateApplicationQuery_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var mockUpdateCommand = new PartiallyUpdateApplicationCommandSql
                {
                    Id = Guid.NewGuid().ToString(),
                    PreferredDepotId = "1"
                };

                var mediator = provider.GetService<IMediator>();

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => mediator.Send<PartiallyUpdateApplicationCommandSql, Unit>(mockUpdateCommand));

                Assert.Equal("Entity not found.", exception.Message);
            }
        }

        private IServiceCollection SetupServiceProvider()
        {
            var context = MockContext();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly);
            serviceCollection.AddSingleton<ILuebenContextSql, LuebenContext>(x => context);
            serviceCollection.AddTransient<IEntityFrameworkPatchSql, EntityFrameworkPatchSql>();
            serviceCollection.AddTransient<IMediator, Mediator>();
            serviceCollection.AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider));
            serviceCollection.AddTransient<IRequestHandler<PartiallyUpdateApplicationCommandSql, Unit>, PartiallyUpdateApplicationCommandHandlerSql>();

            return serviceCollection;
        }

        private LuebenContext MockContext()
        {
            var context = LuebenDbContextFactory.CreateInMemory();

            var application = new Domain.Entities.Application
            {
                Id = _applicationId,
                PreferredDepotId = "1",
                Created = DateTime.UtcNow
            };

            context.Application.AddRange(new List<Domain.Entities.Application> { application });
            context.SaveChanges();

            return context;
        }
    }
}