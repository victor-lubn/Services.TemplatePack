using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Lueben.Microservice.EventHub;
using Lueben.Microservice.Mediator;
using Lueben.Microservice.Serialization;
using Lueben.Templates.Orchestrator.Function.Validation;
using Lueben.Templates.Orchestrator.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Lueben.Templates.Orchestrator.Function.Tests
{
    public class OrchestratorFunctionTests
    {
        private const string TestEventType = "test";
        private readonly Mock<ILogger<OrchestratorFunction>> _loggerMock;
        private readonly Mock<ILogger<EventValidator>> _validatorLoggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IHandlerProvider> _handlerProviderMock;
        private readonly Mock<IEventValidator> _eventValidatorMock;

        public OrchestratorFunctionTests()
        {
            JsonConvert.DefaultSettings = FunctionJsonSerializerSettingsProvider.CreateSerializerSettings;
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<OrchestratorFunction>>();
            _validatorLoggerMock = new Mock<ILogger<EventValidator>>();
            _eventValidatorMock = new Mock<IEventValidator>();
            _handlerProviderMock = new Mock<IHandlerProvider>();
            _handlerProviderMock.Setup(x => x.GetAllNotificationHandlers<Notification<Event<JObject>>>())
                .Returns(new[] { new FakeEventHandler(TestEventType) });
        }

        [Fact]
        public async Task GivenOrchestratorPublish_WhenCorrectEventIsPassed_ThenShouldNotifyHandlers()
        {
            var data = new TestEventData();

            _eventValidatorMock.Setup(x => x.GetValidatedEvent(It.IsAny<string>()))
                .Returns(Task.FromResult(new Event<JObject> { Data = JObject.FromObject(data) }));
            var function = new OrchestratorFunction(_loggerMock.Object, _mediatorMock.Object, _eventValidatorMock.Object);

            await function.EventTrigger(CreateEventData());

            _mediatorMock.Verify(x => x.Publish(It.IsAny<Notification<Event<JObject>>>()), Times.Once);
        }

        [Fact]
        public async Task GivenOrchestratorPublish_WhenEventTypeIsNotProvided_ThenEventIsIgnoredAndIncorrectEventModelLogEventIsCreated()
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Event<TestEventData>
            {
                Type = null,
                Data = new TestEventData()
            }));
            var eventData = new EventData(body);
            var eventValidator = new EventValidator(_validatorLoggerMock.Object, _handlerProviderMock.Object);
            var function = new OrchestratorFunction(_loggerMock.Object, _mediatorMock.Object, eventValidator);

            await function.EventTrigger(eventData);

            _mediatorMock.Verify(x => x.Publish(It.IsAny<Notification<Event<JObject>>>()), Times.Never);
        }

        [Fact]
        public async Task GivenOrchestratorPublish_WhenEventDataIsNotProvided_ThenEventIsIgnoredAndIncorrectEventModelLogEventIsCreated()
        {
            var message = JsonConvert.SerializeObject(new Event<TestEventData>
            {
                Type = TestEventType,
                Data = null
            });
            var body = Encoding.UTF8.GetBytes(message);
            var eventData = new EventData(body);
            var eventValidator = new EventValidator(_validatorLoggerMock.Object, _handlerProviderMock.Object);
            var function = new OrchestratorFunction(_loggerMock.Object, _mediatorMock.Object, eventValidator);

            await function.EventTrigger(eventData);

            _mediatorMock.Verify(x => x.Publish(It.IsAny<Notification<Event<JObject>>>()), Times.Never);
        }

        [Fact]
        public async Task GivenOrchestratorPublish_WhenEventIsNotValidJson_ThenEventIsIgnoredAndIncorrectEventModelLogEventIsCreated()
        {
            var body = Encoding.UTF8.GetBytes("{ type = test, data = test ");
            var eventData = new EventData(body);
            var eventValidator = new EventValidator(_validatorLoggerMock.Object, _handlerProviderMock.Object);
            var function = new OrchestratorFunction(_loggerMock.Object, _mediatorMock.Object, eventValidator);

            await function.EventTrigger(eventData);

            _mediatorMock.Verify(x => x.Publish(It.IsAny<Notification<Event<JObject>>>()), Times.Never);
        }

        [Fact]
        public async Task GivenOrchestratorPublish_WhenEventIsNotValidJson_ThenEventIsIgnoredAndIncorrectEventModelLogEventIsCreated2()
        {
            var body = Encoding.UTF8.GetBytes("{\"Type:\"email,\"RetryCount\":0,\"Data}");
            var eventData = new EventData(body);
            var eventValidator = new EventValidator(_validatorLoggerMock.Object, _handlerProviderMock.Object);
            var function = new OrchestratorFunction(_loggerMock.Object, _mediatorMock.Object, eventValidator);

            await function.EventTrigger(eventData);

            _mediatorMock.Verify(x => x.Publish(It.IsAny<Notification<Event<JObject>>>()), Times.Never);
        }

        [Fact]
        public async Task GivenOrchestratorPublish_WhenEventTypeIsUnknown_ThenEventIsIgnoredAndLogEventIsCreated()
        {
            var message = JsonConvert.SerializeObject(new Event<TestEventData>
            {
                Type = "unknown",
                Data = new TestEventData()
            });
            var body = Encoding.UTF8.GetBytes(message);
            var eventData = new EventData(body);
            var eventValidator = new EventValidator(_validatorLoggerMock.Object, _handlerProviderMock.Object);
            var function = new OrchestratorFunction(_loggerMock.Object, _mediatorMock.Object, eventValidator);

            await function.EventTrigger(eventData);

            _mediatorMock.Verify(x => x.Publish(It.IsAny<Notification<Event<JObject>>>()), Times.Never);
        }

        private static EventData CreateEventData()
        {
            var bodySerialized = JsonConvert.SerializeObject(new Event<TestEventData>());
            var body = Encoding.UTF8.GetBytes(bodySerialized);
            var eventData = new EventData(body);
            return eventData;
        }
    }
}