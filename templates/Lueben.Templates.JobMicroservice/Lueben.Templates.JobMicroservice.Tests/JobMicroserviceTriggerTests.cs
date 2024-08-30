using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lueben.Templates.JobMicroservice.Function.Tests
{
    public class JobMicroserviceTriggerTests
    {
        private readonly Mock<ILogger<JobMicroserviceTriggerFunction>> _mockLogger;
        private readonly Mock<IDurableClient> _durableClientMock;

        public JobMicroserviceTriggerTests()
        {
            _mockLogger = new Mock<ILogger<JobMicroserviceTriggerFunction>>();
            _durableClientMock = new Mock<IDurableClient>();
        }

        [Fact]
        public async Task GivenJobMicroserviceTrigger_WhenFunctionExecutesIt_ThenTheOrchestrationFunctionShouldBeScheduled()
        {
            var triggerFunction = new JobMicroserviceTriggerFunction(_mockLogger.Object);

            await triggerFunction.JobMicroserviceTimeTrigger(null, _durableClientMock.Object);

            _durableClientMock.Verify(x => x.StartNewAsync(nameof(JobMicroserviceFunction.JobMicroserviceOrchestrator), JobMicroserviceTriggerFunction.InstanceId, true));
        }

        [Fact]
        public async Task GivenJobMicroserviceTrigger_WhenAnotherInstanceOfOrchestratorIsRunning_ThenTheOrchestrationFunctionShouldNotBeScheduled()
        {
            _durableClientMock.Setup(x => x.GetStatusAsync(It.IsAny<string>(), false, false, true))
                .Returns(Task.FromResult(new DurableOrchestrationStatus { RuntimeStatus = OrchestrationRuntimeStatus.Running }));
            var triggerFunction = new JobMicroserviceTriggerFunction(_mockLogger.Object);

            await triggerFunction.JobMicroserviceTimeTrigger(null, _durableClientMock.Object);

            _durableClientMock.Verify(x => x.StartNewAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GivenForceJobMicroservice_WhenFunctionExecutesIt_ThenTheOrchestrationFunctionShouldNotBeScheduled()
        {
            var triggerFunction = new JobMicroserviceTriggerFunction(_mockLogger.Object);

            await triggerFunction.ForceJobMicroservice(null, _durableClientMock.Object);

            _durableClientMock.Verify(x => x.StartNewAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GivenForceJobMicroservice_WhenTheInstanceOfFunctionIsAlreadyInUse_ThenTheConflictStatusCodeShouldBeReturned()
        {
            _durableClientMock.Setup(x => x.GetStatusAsync(It.IsAny<string>(), false, false, true))
                .Returns(Task.FromResult(new DurableOrchestrationStatus { RuntimeStatus = OrchestrationRuntimeStatus.Running }));
            var triggerFunction = new JobMicroserviceTriggerFunction(_mockLogger.Object);

            var result = await triggerFunction.ForceJobMicroservice(null, _durableClientMock.Object);

            Assert.IsType<ConflictResult>(result);
        }
    }
}