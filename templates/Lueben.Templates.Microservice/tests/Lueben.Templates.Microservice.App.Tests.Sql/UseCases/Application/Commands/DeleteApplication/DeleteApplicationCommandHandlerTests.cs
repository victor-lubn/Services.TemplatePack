using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.Tests.Sql.Helpers;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.DeleteApplication;
using Lueben.Templates.Microservice.Data.Sql;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Sql.UseCases.Application.Commands.DeleteApplication
{
    public class DeleteApplicationCommandHandlerTests
    {
        private readonly string _applicationId = Guid.NewGuid().ToString();

        [Fact]
        public async Task GivenDeleteApplicationCommand_WhenCommandIsExecuted_ThenEntityAndAllTheChildrenShouldBeDeletedFromTheDatabase()
        {
            var serviceCollection = SetupServiceProvider();

            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mockDeleteCommand = new DeleteApplicationCommandSql
                {
                    Id = _applicationId
                };

                var mediator = provider.GetService<IMediator>();
                await mediator.Send<DeleteApplicationCommandSql, Lueben.Microservice.Mediator.Unit>(mockDeleteCommand);

                var context = provider.GetService<ILuebenContextSql>();

                Assert.Equal(0, context.Application.Count());
            }
        }

        [Fact]
        public async Task GivenDeleteApplicationQuery_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            var serviceCollection = SetupServiceProvider();

            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mockUpdateCommand = new DeleteApplicationCommandSql
                {
                    Id = Guid.NewGuid().ToString()
                };

                var mediator = provider.GetService<IMediator>();

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => mediator.Send<DeleteApplicationCommandSql, Unit>(mockUpdateCommand));

                Assert.Equal("Entity not found.", exception.Message);
            }
        }

        private IServiceCollection SetupServiceProvider()
        {
            var context = MockContext();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly);
            serviceCollection.AddSingleton<ILuebenContextSql, LuebenContext>(x => context);
            serviceCollection.AddTransient<IMediator, Mediator>();
            serviceCollection.AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider));
            serviceCollection.AddTransient<IRequestHandler<DeleteApplicationCommandSql, Unit>, DeleteApplicationCommandHandlerSql>();

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
