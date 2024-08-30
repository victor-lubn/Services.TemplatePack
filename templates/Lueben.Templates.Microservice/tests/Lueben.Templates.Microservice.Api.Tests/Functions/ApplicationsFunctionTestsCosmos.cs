using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lueben.Microservice.Api.ValidationFunction.Exceptions;
using Lueben.Microservice.EntityFunction.Models;
using Lueben.Microservice.Mediator;
using Lueben.Microservice.Serialization;
using Lueben.Templates.Microservice.Api.Functions.v1;
using Lueben.Templates.Microservice.Api.Tests.Extensions;
using Lueben.Templates.Microservice.Api.Validators;
using Lueben.Templates.Microservice.App.Exceptions;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.CreateApplication;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.DeleteApplication;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication;
using Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetApplication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Application = Lueben.Templates.Microservice.Api.Contract.Models.Application;

namespace Lueben.Templates.Microservice.Api.Tests.Functions
{
    public class ApplicationsFunctionTestsCosmos
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public ApplicationsFunctionTestsCosmos()
        {
            JsonConvert.DefaultSettings = FunctionJsonSerializerSettingsProvider.CreateSerializerSettings;

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            _serviceCollection = SetupServiceProvider();
        }

        [Fact]
        public async Task GivenGetApplicationMethod_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send<GetApplicationQueryCosmos, Domain.Entities.Application>(It.IsAny<GetApplicationQueryCosmos>()))
                .Throws<EntityNotFoundException>();

            var request = CreateHttpRequest(false);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => applicationsFunction.GetApplication(request, Guid.NewGuid().ToString()));

                Assert.Equal("Entity not found.", exception.Message);

                mockMediator.Verify(m => m.Send<GetApplicationQueryCosmos, Domain.Entities.Application>(It.IsAny<GetApplicationQueryCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenCreateApplicationMethod_WhenFunctionExecutesIt_ThenNewApplicationShouldBeCreated()
        {
            const string mockPreferredDepotId = "1";
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send<CreateApplicationCommandCosmos, string>(It.IsAny<CreateApplicationCommandCosmos>()))
                .Callback<IRequest<string>>(req =>
                {
                    var command = req as CreateApplicationCommandCosmos;

                    Assert.NotNull(command);
                    Assert.Equal(mockPreferredDepotId, command.PreferredDepotId);
                })
                .Returns(() => Task.FromResult(mockApplicationId));

            var request = CreateHttpRequest(true);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var response = (CreatedObjectResult<string>)await applicationsFunction.CreateApplication(request);

                Assert.Equal(response.StatusCode, (int)HttpStatusCode.Created);
                Assert.Equal(mockApplicationId, response.Data);

                mockMediator.Verify(m => m.Send<CreateApplicationCommandCosmos, string>(It.IsAny<CreateApplicationCommandCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenCreateFullApplicationMethod_WhenFunctionExecutesIt_ThenNewApplicationWithAllChildrenObjectShouldBeCreated()
        {
            const string mockTestText = "test";
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send<CreateApplicationCommandCosmos, string>(It.IsAny<CreateApplicationCommandCosmos>()))
                .Callback<IRequest<string>>(req =>
                {
                    var command = req as CreateApplicationCommandCosmos;

                    Assert.NotNull(command);
                    Assert.Equal(mockTestText, command.PreferredDepotId);
                })
                .Returns(() => Task.FromResult(mockApplicationId));

            var request = CreateHttpRequestWithFullModel();

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var response = (CreatedObjectResult<string>)await applicationsFunction.CreateApplication(request);

                Assert.Equal(response.StatusCode, (int)HttpStatusCode.Created);
                Assert.Equal(mockApplicationId, response.Data);

                mockMediator.Verify(m => m.Send<CreateApplicationCommandCosmos, string>(It.IsAny<CreateApplicationCommandCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenCreateApplicationMethod_WhenTheModelIsNotValid_ThenTheModelNotValidExceptionShouldBeThrown()
        {
            var mockMediator = new Mock<IMediator>();

            var request = CreateHttpRequest(true, false);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var exception = await Assert.ThrowsAsync<ModelNotValidException>(() => applicationsFunction.CreateApplication(request));

                Assert.Equal("Model is not valid.", exception.Message);

                mockMediator.Verify(m => m.Send<CreateApplicationCommandCosmos, string>(It.IsAny<CreateApplicationCommandCosmos>()), Times.Never);
            }
        }

        [Fact]
        public async Task GivenCreateApplicationMethod_WhenTheModelIsEmpty_ThenNewApplicationShouldBeCreated()
        {
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send<CreateApplicationCommandCosmos, string>(It.IsAny<CreateApplicationCommandCosmos>()))
                .Returns(() => Task.FromResult(mockApplicationId));

            var request = CreateHttpRequestWithEmptyModel();

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var response = (CreatedObjectResult<string>)await applicationsFunction.CreateApplication(request);

                Assert.Equal(response.StatusCode, (int)HttpStatusCode.Created);
                Assert.Equal(mockApplicationId, response.Data);

                mockMediator.Verify(m => m.Send<CreateApplicationCommandCosmos, string>(It.IsAny<CreateApplicationCommandCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenPatchApplicationMethod_WhenFunctionExecutesIt_ThenTheApplicationShouldBeUpdated()
        {
            const string mockPreferredDepotId = "1";
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();

            mockMediator
                .Setup(m => m.Send<PartiallyUpdateApplicationCommandCosmos, Unit>(It.IsAny<PartiallyUpdateApplicationCommandCosmos>()))
                .Callback<IRequest<Unit>>(req =>
                {
                    var command = req as PartiallyUpdateApplicationCommandCosmos;

                    Assert.NotNull(command);
                    Assert.Equal(mockPreferredDepotId, command.PreferredDepotId);
                })
                .Returns(() => Task.FromResult(Unit.Value));

            var request = CreateHttpRequest(true);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var response = (NoContentResult)await applicationsFunction.PatchApplication(request, mockApplicationId);

                Assert.Equal((int)HttpStatusCode.NoContent, response.StatusCode);

                mockMediator.Verify(m => m.Send<PartiallyUpdateApplicationCommandCosmos, Unit>(It.IsAny<PartiallyUpdateApplicationCommandCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenPatchApplicationMethod_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send<PartiallyUpdateApplicationCommandCosmos, Unit>(It.IsAny<PartiallyUpdateApplicationCommandCosmos>()))
                .Throws<EntityNotFoundException>();

            var request = CreateHttpRequest(true);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => applicationsFunction.PatchApplication(request, mockApplicationId));

                Assert.Equal("Entity not found.", exception.Message);

                mockMediator.Verify(m => m.Send<PartiallyUpdateApplicationCommandCosmos, Unit>(It.IsAny<PartiallyUpdateApplicationCommandCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenPatchApplicationMethod_WhenTheModelIsNotValid_ThenTheModelNotValidExceptionShouldBeThrown()
        {
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();

            var request = CreateHttpRequest(true, false);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var exception = await Assert.ThrowsAsync<ModelNotValidException>(() => applicationsFunction.PutApplication(request, mockApplicationId));

                Assert.Equal("Model is not valid.", exception.Message);

                mockMediator.Verify(m => m.Send<PartiallyUpdateApplicationCommandCosmos, string>(It.IsAny<PartiallyUpdateApplicationCommandCosmos>()), Times.Never);
            }
        }

        [Fact]
        public async Task GivenPutApplicationMethod_WhenFunctionExecutesIt_ThenTheApplicationShouldBeUpdated()
        {
            const string mockPreferredDepotId = "1";
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send<UpdateApplicationCommandCosmos, Unit>(It.IsAny<UpdateApplicationCommandCosmos>()))
                .Callback<IRequest<Unit>>(req =>
                {
                    var command = req as UpdateApplicationCommandCosmos;

                    Assert.NotNull(command);
                    Assert.Equal(mockPreferredDepotId, command.PreferredDepotId);
                })
                .Returns(() => Task.FromResult(Unit.Value));

            var request = CreateHttpRequest(true);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var response = (NoContentResult)await applicationsFunction.PutApplication(request, mockApplicationId);

                Assert.Equal((int)HttpStatusCode.NoContent, response.StatusCode);

                mockMediator.Verify(m => m.Send<UpdateApplicationCommandCosmos, Unit>(It.IsAny<UpdateApplicationCommandCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenPutApplicationMethod_WhenTheModelIsNotValid_ThenTheModelNotValidExceptionShouldBeThrown()
        {
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();

            var request = CreateHttpRequest(true, false);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var exception = await Assert.ThrowsAsync<ModelNotValidException>(() => applicationsFunction.PutApplication(request, mockApplicationId));

                Assert.Equal("Model is not valid.", exception.Message);

                mockMediator.Verify(m => m.Send<UpdateApplicationCommandCosmos, string>(It.IsAny<UpdateApplicationCommandCosmos>()), Times.Never);
            }
        }

        [Fact]
        public async Task GivenPutApplicationMethod_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send<UpdateApplicationCommandCosmos, Unit>(It.IsAny<UpdateApplicationCommandCosmos>()))
                .Throws<EntityNotFoundException>();

            var request = CreateHttpRequest(true);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => applicationsFunction.PutApplication(request, mockApplicationId));

                Assert.Equal("Entity not found.", exception.Message);

                mockMediator.Verify(m => m.Send<UpdateApplicationCommandCosmos, Unit>(It.IsAny<UpdateApplicationCommandCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenDeleteApplicationMethod_WhenFunctionExecutesIt_ThenTheApplicationShouldBeDeleted()
        {
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send<DeleteApplicationCommandCosmos, Unit>(It.IsAny<DeleteApplicationCommandCosmos>()))
                .Callback<IRequest<Unit>>(req =>
                {
                    var command = req as DeleteApplicationCommandCosmos;

                    Assert.NotNull(command);
                    Assert.Equal(mockApplicationId, command.Id);
                })
                .Returns(() => Task.FromResult(Unit.Value));

            var request = CreateHttpRequest(false);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var response = (NoContentResult)await applicationsFunction.DeleteApplication(request, mockApplicationId);

                Assert.Equal((int)HttpStatusCode.NoContent, response.StatusCode);

                mockMediator.Verify(m => m.Send<DeleteApplicationCommandCosmos, Unit>(It.IsAny<DeleteApplicationCommandCosmos>()), Times.Once);
            }
        }

        [Fact]
        public async Task GivenDeleteApplicationMethod_WhenTheApplicationDoesNotExist_ThenTheEntityNotFoundExceptionShouldBeThrown()
        {
            string mockApplicationId = Guid.NewGuid().ToString();

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send<DeleteApplicationCommandCosmos, Unit>(It.IsAny<DeleteApplicationCommandCosmos>()))
                .Throws<EntityNotFoundException>();

            var request = CreateHttpRequest(false);

            using (var provider = _serviceCollection.BuildServiceProvider())
            {
                var applicationsFunction = CreateApplicationsFunction(mockMediator, provider, request);

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => applicationsFunction.DeleteApplication(request, mockApplicationId));

                Assert.Equal("Entity not found.", exception.Message);

                mockMediator.Verify(m => m.Send<DeleteApplicationCommandCosmos, Unit>(It.IsAny<DeleteApplicationCommandCosmos>()), Times.Once);
            }
        }

        private static IServiceCollection SetupServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile.AutoMapperProfile).Assembly, typeof(App.AutoMapperProfile.AutoMapperProfile).Assembly);

            return serviceCollection;
        }

        private ApplicationsFunctionCosmos CreateApplicationsFunction(Mock<IMediator> mockMediator, ServiceProvider provider, HttpRequest request)
        {
            _mockHttpContextAccessor.Setup(c => c.HttpContext.Request).Returns(request);
            var applicationsFunction = new ApplicationsFunctionCosmos(mockMediator.Object, provider.GetService<IMapper>(), _mockHttpContextAccessor.Object, new ApplicationModelValidator());
            return applicationsFunction;
        }

        public static HttpRequest CreateHttpRequest(bool hasBody, bool isModelValid = true, bool hasQueryParams = false)
        {
            var context = new DefaultHttpContext();
            var request = context.Request;
            if (hasBody)
            {
                var json = JsonConvert.SerializeObject(new Application
                {
                    PreferredDepotId = isModelValid ? "1" : new string('-', 52)
                });
                request.Body = json.GenerateStreamFromString();
            }

            if (hasQueryParams)
            {
                request.QueryString = new QueryString("?companyName=Test");
            }

            return request;
        }

        public static HttpRequest CreateHttpRequestWithFullModel()
        {
            var context = new DefaultHttpContext();
            var request = context.Request;
            var json = JsonConvert.SerializeObject(new Application
            {
                PreferredDepotId = "test"
            });
            request.Body = json.GenerateStreamFromString();

            return request;
        }

        public static HttpRequest CreateHttpRequestWithEmptyModel()
        {
            var context = new DefaultHttpContext();
            var request = context.Request;
            var json = JsonConvert.SerializeObject(new Application());
            request.Body = json.GenerateStreamFromString();

            return request;
        }
    }
}
