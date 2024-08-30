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
    public class PartiallyUpdateApplicationCosmosCommandHandlerTests
    {
        [Fact]
        public async Task GivenPartiallyUpdateApplicationCommand_WhenCommandIsExecuted_ThenEntityShouldBeUpdatedInTheDatabase()
        {
            var mockUpdateCommand = new PartiallyUpdateApplicationCommandCosmos
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
                })
                .Returns(Task.FromResult(new Domain.Entities.Application()));

            var mockPartiallyUpdateService = new Mock<IPartiallyUpdateServiceCosmos<Domain.Entities.Application, PartiallyUpdateApplicationCommandCosmos>>();
            mockPartiallyUpdateService.Setup(x => x.ApplyChanges(It.IsAny<Domain.Entities.Application>(), It.IsAny<PartiallyUpdateApplicationCommandCosmos>()))
                .Callback<Domain.Entities.Application, PartiallyUpdateApplicationCommandCosmos>((app, command) =>
                {
                    Assert.NotNull(app);
                    Assert.NotNull(command);
                })
                .Returns(Task.CompletedTask);

            var serviceCollection = SetupServiceProvider(mockRepository, mockPartiallyUpdateService);

            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mediator = provider.GetService<IMediator>();
                await mediator.Send<PartiallyUpdateApplicationCommandCosmos, Unit>(mockUpdateCommand);

                mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Application>(), It.IsAny<PartitionKey>()), Times.Once);
                mockPartiallyUpdateService.Verify(x => x.ApplyChanges(It.IsAny<Domain.Entities.Application>(), It.IsAny<PartiallyUpdateApplicationCommandCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenPartiallyUpdateApplicationQuery_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            var mockUpdateCommand = new PartiallyUpdateApplicationCommandCosmos
            {
                Id = Guid.NewGuid().ToString(),
                PreferredDepotId = "2"
            };

            var mockApplication = new List<Domain.Entities.Application>();

            var mockRepository = new Mock<IRepositoryCosmos<Domain.Entities.Application>>();
            mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Application, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<IEnumerable<Domain.Entities.Application>>(Task.FromResult(mockApplication.AsEnumerable())));

            var mockPartiallyUpdateService = new Mock<IPartiallyUpdateServiceCosmos<Domain.Entities.Application, PartiallyUpdateApplicationCommandCosmos>>();

            var serviceCollection = SetupServiceProvider(mockRepository, mockPartiallyUpdateService);

            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mediator = provider.GetService<IMediator>();

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => mediator.Send<PartiallyUpdateApplicationCommandCosmos, Unit>(mockUpdateCommand));

                Assert.Equal("Entity not found.", exception.Message);

                mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Application>(), It.IsAny<PartitionKey>()), Times.Never);
                mockPartiallyUpdateService.Verify(x => x.ApplyChanges(It.IsAny<Domain.Entities.Application>(), It.IsAny<PartiallyUpdateApplicationCommandCosmos>()), Times.Never);
            }
        }

        private static IServiceCollection SetupServiceProvider(Mock<IRepositoryCosmos<Domain.Entities.Application>> mockRepository, Mock<IPartiallyUpdateServiceCosmos<Domain.Entities.Application, PartiallyUpdateApplicationCommandCosmos>> mockPartiallyUpdateService)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly)
                             .AddTransient<IMediator, Mediator>()
                             .AddTransient(provider => mockRepository.Object)
                             .AddTransient(provider => mockPartiallyUpdateService.Object)
                             .AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider))
                             .AddTransient<IRequestHandler<PartiallyUpdateApplicationCommandCosmos, Unit>, PartiallyUpdateApplicationCommandHandlerCosmos>();

            return serviceCollection;
        }
    }
}
