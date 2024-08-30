using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using Lueben.Microservice.RestSharpClient;
using Lueben.Microservice.RestSharpClient.Abstractions;
using Lueben.Templates.ApiClient.Models;
using Moq;
using RestSharp;

namespace Lueben.Templates.ApiClient.Tests
{
    public class ApiClientTests
    {
        private const string BaseUrl = "http://localhost:7072";
        private readonly Mock<IRestSharpClient> _restSharpClientMock;
        private readonly ApiClient _client;
        private readonly IFixture _fixture;

        public ApiClientTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var options = new ApiOptions { BaseUrl = BaseUrl };
            _fixture.Inject(options);

            var restSharpClientFactoryMock = new Mock<IRestSharpClientFactory>();
            _restSharpClientMock = new Mock<IRestSharpClient>();
            restSharpClientFactoryMock.Setup(x => x.Create(It.IsAny<ApiClient>())).Returns(_restSharpClientMock.Object);
            _fixture.Inject(restSharpClientFactoryMock.Object);

            _client = _fixture.Create<ApiClient>();
        }

        [Fact]
        public async Task GivenAddAsync_WhenCalled_ThenPostRequestShouldBeSent()
        {
            var entity = _fixture.Create<Entity>();
            await _client.AddAsync(entity);

            _restSharpClientMock.Verify(
                x => x.ExecuteRequestAsync<object>(It.Is<RestRequest>(r =>
                    r.Method == Method.Post && r.Resource == $"{BaseUrl}/entity" &&
                    r.Parameters.Any(p => p.Value == entity && p.Type == ParameterType.RequestBody))),
                Times.Once);
        }

        [Fact]
        public async Task GivenAddAsync_WhenRestClientApiException_ThenShouldReThrowTheException()
        {
            var entity = _fixture.Create<Entity>();
            var exception = new RestClientApiException("service", "response", HttpStatusCode.InternalServerError, null);
            _restSharpClientMock.Setup(
                x => x.ExecuteRequestAsync<object>(It.IsAny<RestRequest>())).Throws(exception);

            var reThrownException = await Assert.ThrowsAsync<RestClientApiException>(async () =>
                await _client.AddAsync(entity));

            Assert.Equal(exception.ResponseContent, reThrownException.ResponseContent);

            _restSharpClientMock.Verify(
                x => x.ExecuteRequestAsync<object>(It.Is<RestRequest>(r =>
                    r.Method == Method.Post && r.Resource == $"{BaseUrl}/entity" &&
                    r.Parameters.Any(p => p.Value == entity && p.Type == ParameterType.RequestBody))),
                Times.Once);
        }

        [Fact]
        public async Task GivenAddAsync_WhenException_ThenShouldReThrowTheException()
        {
            var entity = _fixture.Create<Entity>();
            var exception = new Exception("test exception message");
            _restSharpClientMock.Setup(
                x => x.ExecuteRequestAsync<object>(It.IsAny<RestRequest>())).Throws(exception);

            var reThrownException = await Assert.ThrowsAsync<Exception>(async () =>
                await _client.AddAsync(entity));

            Assert.Equal(exception.Message, reThrownException.Message);

            _restSharpClientMock.Verify(
                x => x.ExecuteRequestAsync<object>(It.Is<RestRequest>(r =>
                    r.Method == Method.Post && r.Resource == $"{BaseUrl}/entity" &&
                    r.Parameters.Any(p => p.Value == entity && p.Type == ParameterType.RequestBody))),
                Times.Once);
        }
    }
}