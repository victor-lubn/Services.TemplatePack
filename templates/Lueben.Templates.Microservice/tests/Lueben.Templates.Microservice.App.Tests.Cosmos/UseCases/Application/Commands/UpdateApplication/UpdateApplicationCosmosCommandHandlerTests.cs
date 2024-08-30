using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Cosmos.UseCases.Application.Commands.UpdateApplication
{
    public class UpdateApplicationCosmosCommandHandlerTests
    {
        [Fact]
        public async Task GivenUpdateApplicationCommand_WhenCommandIsExecuted_ThenEntityShouldBeUpdatedInTheDatabase()
        {
            var mockUpdateCommand = new UpdateApplicationCommandCosmos
            {
                Id = Guid.NewGuid().ToString(),
                PreferredDepotId = "2"
            };

            var mockApplication = new List<Domain.Entities.Application>
            {
                new Domain.Entities.Application()
                {
                    Id = Guid.NewGuid().ToString()
                }
            };

            var mockRepository = new Mock<IRepositoryCosmos<Domain.Entities.Application>>();
            mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Application, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<IEnumerable<Domain.Entities.Application>>(Task.FromResult(mockApplication.AsEnumerable())));
            mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Application>(), It.IsAny<PartitionKey>()))
                .Callback<Domain.Entities.Application, PartitionKey>((app, key) =>
                {
                    Assert.NotNull(app);
                    Assert.Equal(app.Id, mockUpdateCommand.Id);
                })
                .Returns(Task.FromResult(new Domain.Entities.Application()));

            var serviceCollection = SetupServiceProvider(mockRepository);

            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mediator = provider.GetService<IMediator>();
                await mediator.Send<UpdateApplicationCommandCosmos, Unit>(mockUpdateCommand);

                mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Application>(), It.IsAny<PartitionKey>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenUpdateApplicationQuery_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            var mockUpdateCommand = new UpdateApplicationCommandCosmos
            {
                Id = Guid.NewGuid().ToString(),
                PreferredDepotId = "1"
            };

            var mockApplication = new List<Domain.Entities.Application>();

            var mockRepository = new Mock<IRepositoryCosmos<Domain.Entities.Application>>();
            mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Application, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<IEnumerable<Domain.Entities.Application>>(Task.FromResult(mockApplication.AsEnumerable())));

            var serviceCollection = SetupServiceProvider(mockRepository);

            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mediator = provider.GetService<IMediator>();

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => mediator.Send<UpdateApplicationCommandCosmos, Unit>(mockUpdateCommand));

                Assert.Equal("Entity not found.", exception.Message);

                mockRepository.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        private static IServiceCollection SetupServiceProvider(Mock<IRepositoryCosmos<Domain.Entities.Application>> mockRepository)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly)
                             .AddTransient<IMediator, Mediator>()
                             .AddTransient(provider => mockRepository.Object)
                             .AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider))
                             .AddTransient<IRequestHandler<UpdateApplicationCommandCosmos, Unit>, UpdateApplicationCommandHandlerCosmos>();

            return serviceCollection;
        }
    }
}
