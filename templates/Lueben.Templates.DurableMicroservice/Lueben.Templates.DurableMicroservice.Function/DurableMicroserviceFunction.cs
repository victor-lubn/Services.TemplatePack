using System;
using System.Threading.Tasks;
using Lueben.Microservice.ApplicationInsights;
using Lueben.Microservice.DurableFunction;
using Lueben.Microservice.DurableFunction.Exceptions;
using Lueben.Microservice.DurableFunction.Extensions;
using Lueben.Templates.Eventing.Clients.Stub;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lueben.Templates.DurableMicroservice.Function
{
    public class DurableMicroserviceFunction : OrchestratorFunction<DurableMicroserviceData>
    {
        private readonly IOptionsSnapshot<WorkflowOptions> _workflowOptions;
        private readonly IDependencyOneClient _dependencyOneClient;
        private readonly IDependencyTwoClient _dependencyTwoClient;

        public DurableMicroserviceFunction(
            TelemetryConfiguration telemetryConfiguration,
            ILogger<OrchestratorFunction<DurableMicroserviceData>> logger,
            ILoggerService loggerService,
            IOptionsSnapshot<WorkflowOptions> workflowOptions,
            IDependencyOneClient dependencyOneClient,
            IDependencyTwoClient dependencyTwoClient)
            : base(telemetryConfiguration, logger, loggerService)
        {
            _workflowOptions = workflowOptions;
            _dependencyOneClient = dependencyOneClient;
            _dependencyTwoClient = dependencyTwoClient;
        }

        [FunctionName(nameof(DurableMicroserviceOrchestrator))]
        public async Task DurableMicroserviceOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            await HandleErrors(context);
        }

        public override async Task ProcessActivities(IDurableOrchestrationContext context, DurableMicroserviceData eventData)
        {
            var data = context.GetInput<DurableMicroserviceData>();

            await context.CallDurableActivity(nameof(StepOneActivity), data, _workflowOptions.Value);
            await context.CallDurableActivity(nameof(StepTwoActivity), data, _workflowOptions.Value);
        }

        [FunctionName(nameof(StepOneActivity))]
        public async Task StepOneActivity([ActivityTrigger] DurableMicroserviceData eventData)
        {
            try
            {
                await _dependencyOneClient.GetPIIDataForStepOne(eventData.ApplicationId);
                await _dependencyTwoClient.GetPIIDataForStepTwo(eventData.ApplicationId);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new IncorrectEventDataException("Failed to get application data.", e);
            }
            catch (Exception e)
            {
                var message = "Failed to get data for step two.";
                Logger.LogError(e, message);
                throw new EventDataProcessFailureException(message, e);
            }

            try
            {
                await _dependencyOneClient.ExecuteStepOne();
            }
            catch (Exception e)
            {
                var message = "Failed to execute step two.";
                Logger.LogError(e, message);
                throw new EventDataProcessFailureException(message, e);
            }
        }

        [FunctionName(nameof(StepTwoActivity))]
        public async Task StepTwoActivity([ActivityTrigger] DurableMicroserviceData eventData)
        {
            try
            {
                await _dependencyTwoClient.GetPIIDataForStepTwo(eventData.ApplicationId);
            }
            catch (Exception ex)
            {
                var message = "Failed to get data for step two.";
                Logger.LogError(ex, message);
                throw new EventDataProcessFailureException(message, ex);
            }

            try
            {
                await _dependencyTwoClient.ExecuteStepTwo();
            }
            catch (Exception ex)
            {
                var message = "Failed to execute step two.";
                Logger.LogError(ex, message);
                throw new EventDataProcessFailureException(message, ex);
            }
        }
    }
}