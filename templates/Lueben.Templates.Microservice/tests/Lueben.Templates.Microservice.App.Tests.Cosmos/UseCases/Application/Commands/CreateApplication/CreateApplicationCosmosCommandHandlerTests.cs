using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Mediator;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.CreateApplication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Lueben.Templates.Microservice.App.Tests.Cosmos.UseCases.Application.Commands.CreateApplication
{
    public class CreateApplicationCosmosCommandHandlerTests
    {
        [Fact]
        public async Task GivenCreateApplicationCommand_WhenCommandIsExecuted_ThenEntityAndApplicationEventShouldBeSavedToDatabase()
        {
            var mockApplication = new Domain.Entities.Application()
            {
                Id = Guid.NewGuid().ToString()
            };

            var createCommand = new CreateApplicationCommandCosmos()
            {
                PreferredDepotId = "1"
            };

            var mockRepository = new Mock<IRepositoryCosmos<Domain.Entities.Application>>();
            mockRepository.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Application>(), It.IsAny<CancellationToken>()))
                .Callback<Domain.Entities.Application, CancellationToken>((app, token) =>
                {
                    Assert.NotNull(app);
                    Assert.Equal(app.PreferredDepotId, createCommand.PreferredDepotId);
                })
                .Returns(new ValueTask<Domain.Entities.Application>(Task.FromResult(mockApplication)));

            var serviceCollection = SetupServiceProvider(mockRepository);
            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mediator = provider.GetService<IMediator>();
                var applicationId = await mediator.Send<CreateApplicationCommandCosmos, string>(createCommand);

                Assert.Equal(mockApplication.Id, applicationId);

                mockRepository.Verify(x => x.CreateAsync(It.IsAny<Domain.Entities.Application>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenCreateApplicationCommand_WhenCommandIsExecutedAndApplicationModelIsEmpty_ThenEntityAndApplicationEventShouldBeSavedToDatabase()
        {
            var mockApplication = new Domain.Entities.Application()
            {
                Id = Guid.NewGuid().ToString()
            };

            var createCommand = new CreateApplicationCommandCosmos();

            var mockRepository = new Mock<IRepositoryCosmos<Domain.Entities.Application>>();
            mockRepository.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Application>(), It.IsAny<CancellationToken>()))
                .Callback<Domain.Entities.Application, CancellationToken>((app, token) =>
                {
                    Assert.NotNull(app);
                    Assert.NotNull(app.Id);
                    Assert.Null(app.PreferredDepotId);
                    Assert.Null(app.RejectionReason);
                    Assert.Null(app.Comments);
                })
                .Returns(new ValueTask<Domain.Entities.Application>(Task.FromResult(mockApplication)));

            var serviceCollection = SetupServiceProvider(mockRepository);
            using (var provider = serviceCollection.BuildServiceProvider())
            {
                var mediator = provider.GetService<IMediator>();
                var applicationId = await mediator.Send<CreateApplicationCommandCosmos, string>(createCommand);

                Assert.Equal(mockApplication.Id, applicationId);

                mockRepository.Verify(x => x.CreateAsync(It.IsAny<Domain.Entities.Application>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        private IServiceCollection SetupServiceProvider(Mock<IRepositoryCosmos<Domain.Entities.Application>> mockRepository)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly)
                .AddTransient<IMediator, Mediator>()
                .AddTransient(provider => mockRepository.Object)
                .AddTransient<IHandlerProvider, HandlerProvider>(provider => new HandlerProvider(provider))
                .AddTransient<IRequestHandler<CreateApplicationCommandCosmos, string>, CreateApplicationCommandHandlerCosmos>();

            return serviceCollection;
        }
    }
}
