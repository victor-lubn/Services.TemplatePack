using System;
using System.Threading.Tasks;
using DurableTask.Core;
using Lueben.Microservice.ApplicationInsights;
using Lueben.Microservice.DurableFunction;
using Lueben.Microservice.DurableFunction.Exceptions;
using Lueben.Templates.Eventing.Clients.Stub;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using RetryOptions = Microsoft.Azure.WebJobs.Extensions.DurableTask.RetryOptions;

namespace Lueben.Templates.DurableMicroservice.Function.Tests
{
    public class DurableMicroserviceFunctionTests
    {
        private const long AppIdMock = 1;
        private readonly Mock<IDependencyOneClient> _dependencyOneClient;
        private readonly Mock<IDependencyTwoClient> _dependencyTwoClient;
        private readonly Mock<IDurableOrchestrationContext> _durableOrchestrationContextMock;
        private readonly Mock<IOptionsSnapshot<WorkflowOptions>> _workFlowOptionsSnapshotMock;
        private readonly Mock<ILogger<DurableMicroserviceFunction>> _loggerMock;
        private readonly Mock<ILoggerService> _loggerServiceMock;

        public DurableMicroserviceFunctionTests()
        {
            _loggerMock = new Mock<ILogger<DurableMicroserviceFunction>>();
            _dependencyOneClient = new Mock<IDependencyOneClient>();
            _dependencyTwoClient = new Mock<IDependencyTwoClient>();
            _workFlowOptionsSnapshotMock = new Mock<IOptionsSnapshot<WorkflowOptions>>();
            _workFlowOptionsSnapshotMock.Setup(x => x.Value)
                .Returns(new WorkflowOptions { MaxEventRetryCount = 2 });
            _durableOrchestrationContextMock = new Mock<IDurableOrchestrationContext>();
            _durableOrchestrationContextMock.Setup(x => x.GetInput<DurableMicroserviceData>())
                .Returns(new DurableMicroserviceData { ApplicationId = AppIdMock });
            _loggerServiceMock = new Mock<ILoggerService>();

            CorrelationTraceContext.Current = new W3CTraceContext();
        }

        [Fact]
        public async Task GivenStepTwoActivity_WhenGetDataIsSuccessful_ThenStepTwoModifyMethodIsExecuted()
        {
            _dependencyTwoClient.Setup(m => m.GetPIIDataForStepTwo(AppIdMock))
                .Returns(() => Task.FromResult("data"));
            var handler = CreateFunction();

            await handler.StepTwoActivity(new DurableMicroserviceData { ApplicationId = AppIdMock });

            _dependencyTwoClient.Verify(m => m.GetPIIDataForStepTwo(AppIdMock), Times.Once);
            _dependencyTwoClient.Verify(m => m.ExecuteStepTwo(), Times.Once);
        }

        [Fact]
        public async Task GivenStepTwoActivityActivity_WhenExecuteStepTwoFails_ThenEventDataProcessFailureIsRaised()
        {
            _dependencyTwoClient.Setup(m => m.ExecuteStepTwo())
                .Throws<Exception>();
            var handler = CreateFunction();

            await Assert.ThrowsAsync<EventDataProcessFailureException>(async () => await handler.StepTwoActivity(new DurableMicroserviceData { ApplicationId = AppIdMock }));

            _dependencyTwoClient.Verify(m => m.GetPIIDataForStepTwo(AppIdMock), Times.Once);
            _dependencyTwoClient.Verify(m => m.ExecuteStepTwo(), Times.Once);
        }

        [Fact]
        public async Task GivenStepTwoActivityActivity_WhenGetPIIDataForStepTwoFails_ThenEventDataProcessFailureIsRaised()
        {
            _dependencyTwoClient.Setup(m => m.GetPIIDataForStepTwo(AppIdMock))
                .Throws<Exception>();
            var handler = CreateFunction();

            await Assert.ThrowsAsync<EventDataProcessFailureException>(async () => await handler.StepTwoActivity(new DurableMicroserviceData { ApplicationId = AppIdMock }));

            _dependencyTwoClient.Verify(m => m.GetPIIDataForStepTwo(AppIdMock), Times.Once);
            _dependencyTwoClient.Verify(m => m.ExecuteStepTwo(), Times.Never);
        }

        [Fact]
        public async Task GivenStepOneActivity_WhenApplicationDataIsIncorrect_ThenEventDataProcessFailureIsRaisedAndStepOneIsNotExecuted()
        {
            _dependencyOneClient.Setup(m => m.GetPIIDataForStepOne(AppIdMock))
                .Throws<ArgumentOutOfRangeException>();
            var handler = CreateFunction();

            await Assert.ThrowsAsync<IncorrectEventDataException>(async () => await handler.StepOneActivity(new DurableMicroserviceData { ApplicationId = AppIdMock }));

            _dependencyOneClient.Verify(m => m.GetPIIDataForStepOne(AppIdMock), Times.Once);
            _dependencyOneClient.Verify(m => m.ExecuteStepOne(), Times.Never);
        }

        [Fact]
        public async Task GivenStepOneActivity_WhenStepOneFails_ThenEventDataProcessFailureExceptionIsRaised()
        {
            _dependencyOneClient.Setup(m => m.ExecuteStepOne()).Throws<Exception>();
            var handler = CreateFunction();

            await Assert.ThrowsAsync<EventDataProcessFailureException>(() => handler.StepOneActivity(new DurableMicroserviceData { ApplicationId = AppIdMock }));

            _dependencyOneClient.Verify(m => m.GetPIIDataForStepOne(AppIdMock), Times.Once);
            _dependencyOneClient.Verify(m => m.ExecuteStepOne(), Times.Once);
        }

        [Fact]
        public async Task GivenDurableMicroserviceOrchestrator_WhenStepOneSucceeded_ThenStepTwoActivityStarted()
        {
            var handler = CreateFunction();
            var input = _durableOrchestrationContextMock.Object.GetInput<DurableMicroserviceData>();

            await handler.DurableMicroserviceOrchestrator(_durableOrchestrationContextMock.Object);

            _durableOrchestrationContextMock.Verify(c => c.CallActivityWithRetryAsync(nameof(DurableMicroserviceFunction.StepOneActivity), It.IsAny<RetryOptions>(), input), Times.Once);
            _durableOrchestrationContextMock.Verify(c => c.CallActivityWithRetryAsync(nameof(DurableMicroserviceFunction.StepTwoActivity), It.IsAny<RetryOptions>(), input), Times.Once);
        }

        [Fact]
        public async Task GivenDurableMicroserviceOrchestrator_WhenStepOneActivityFailedWithIncorrectEventDataException_ThenStepTwoActivityNotStartedButOrchestrationIsSucceded()
        {
            var handler = CreateFunction();
            var input = _durableOrchestrationContextMock.Object.GetInput<DurableMicroserviceData>();
            _durableOrchestrationContextMock.Setup(c =>
                    c.CallActivityWithRetryAsync(nameof(DurableMicroserviceFunction.StepOneActivity), It.IsAny<RetryOptions>(), input))
                .Throws(new IncorrectEventDataException("error"));

            await Assert.ThrowsAsync<IncorrectEventDataException>(async () => await handler.DurableMicroserviceOrchestrator(_durableOrchestrationContextMock.Object));

            _durableOrchestrationContextMock.Verify(c => c.CallActivityWithRetryAsync(nameof(DurableMicroserviceFunction.StepOneActivity), It.IsAny<RetryOptions>(), input), Times.Once);
            _durableOrchestrationContextMock.Verify(c => c.CallActivityWithRetryAsync(nameof(DurableMicroserviceFunction.StepTwoActivity), It.IsAny<RetryOptions>(), input), Times.Never);
        }

        [Fact]
        public async Task GivenDurableMicroservice_WhenStepOneActivityFailedWithEventDataProcessFailureException_ThenOrchestrationIsFailed()
        {
            var handler = CreateFunction();
            var input = _durableOrchestrationContextMock.Object.GetInput<DurableMicroserviceData>();
            _durableOrchestrationContextMock.Setup(c =>
                    c.CallActivityWithRetryAsync(nameof(DurableMicroserviceFunction.StepOneActivity), It.IsAny<RetryOptions>(), input))
                .Throws(new EventDataProcessFailureException("error"));

            await Assert.ThrowsAsync<EventDataProcessFailureException>(async () => await handler.DurableMicroserviceOrchestrator(_durableOrchestrationContextMock.Object));

            _durableOrchestrationContextMock.Verify(c => c.CallActivityWithRetryAsync(nameof(DurableMicroserviceFunction.StepOneActivity), It.IsAny<RetryOptions>(), input), Times.Once);
            _durableOrchestrationContextMock.Verify(c => c.CallActivityWithRetryAsync(nameof(DurableMicroserviceFunction.StepTwoActivity), It.IsAny<RetryOptions>(), input), Times.Never);
        }

        private DurableMicroserviceFunction CreateFunction()
        {
            return new DurableMicroserviceFunction(
                TelemetryConfiguration.CreateDefault(),
                _loggerMock.Object,
                _loggerServiceMock.Object,
                _workFlowOptionsSnapshotMock.Object,
                _dependencyOneClient.Object,
                _dependencyTwoClient.Object);
        }
    }
}