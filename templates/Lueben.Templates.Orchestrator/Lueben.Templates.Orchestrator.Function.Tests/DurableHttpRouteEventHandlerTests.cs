using System.Threading.Tasks;
using Lueben.Microservice.DurableFunction;
using Lueben.Microservice.DurableHttpRouteFunction;
using Lueben.Microservice.EventHub;
using Lueben.Templates.Orchestrator.Function.Handlers;
using Lueben.Templates.Orchestrator.Handlers;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Lueben.Templates.Orchestrator.Function.Tests
{
    public class DurableHttpRouteEventHandlerTests
    {
        private const string EventType = "EventType";
        private readonly Mock<ILogger<DurableHttpRouteEventHandler>> _mockLogger;
        private readonly Mock<IDurableClientFactory> _durableClientFactoryMock;
        private readonly IOptions<DurableTaskOptions> _durableTaskOptions;
        private readonly Mock<IDurableClient> _durableClientMock;
        private readonly Mock<IOptionsSnapshot<HttpEventHandlerOptions>> _httpEventHandlerOptionsMock;

        public DurableHttpRouteEventHandlerTests()
        {
            _mockLogger = new Mock<ILogger<DurableHttpRouteEventHandler>>();
            _durableClientMock = new Mock<IDurableClient>();
            _durableClientFactoryMock = new Mock<IDurableClientFactory>();
            _durableClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<DurableClientOptions>()))
                .Returns(_durableClientMock.Object);
            _durableTaskOptions = Options.Create(new DurableTaskOptions { HubName = "test" });
            _httpEventHandlerOptionsMock = new Mock<IOptionsSnapshot<HttpEventHandlerOptions>>();
            _httpEventHandlerOptionsMock.Setup(x => x.Get(EventType))
                .Returns(new HttpEventHandlerOptions { ServiceUrl = "test" });
        }

        [Fact]
        public async Task GivenDurableHttpRouteEventHandler_WhenEventTypeIsApplicable_ThenOrchestrationIsCreated()
        {
            var handler = new DurableHttpRouteEventHandler(_mockLogger.Object, _httpEventHandlerOptionsMock.Object, _durableClientFactoryMock.Object, _durableTaskOptions);

            await handler.Handle(new Notification<Event<JObject>>
            {
                Data = new Event<JObject>
                {
                    Type = EventType,
                    Data = new JObject()
                }
            });

            _durableClientMock.Verify(x => x.StartNewAsync(nameof(DurableHttpRouteFunction.Orchestrator), It.IsAny<RetryData<HttpRouteInput>>()), Times.Once);
        }

        [Fact]
        public async Task GivenDurableHttpRouteEventHandler_WhenEventTypeIsNotApplicable_ThenOrchestrationIsNotCreated()
        {
            var handler = new DurableHttpRouteEventHandler(_mockLogger.Object, _httpEventHandlerOptionsMock.Object, _durableClientFactoryMock.Object, _durableTaskOptions);

            await handler.Handle(new Notification<Event<JObject>>
            {
                Data = new Event<JObject>
                {
                    Type = "WrongType",
                    Data = new JObject()
                }
            });

            _durableClientMock.Verify(x => x.StartNewAsync(nameof(DurableHttpRouteFunction.Orchestrator), It.IsAny<RetryData<HttpRouteInput>>()), Times.Never);
        }
    }
}
