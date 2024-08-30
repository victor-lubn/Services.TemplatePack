using System;
using System.Threading.Tasks;
using Lueben.Templates.JobMicroservice.Function.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Lueben.Templates.JobMicroservice.Function
{
    /// <summary>
    /// Two kind of triggers.
    /// 1) By timer - the main one
    /// 2) By http request to support automated and manual tests.
    ///
    /// Job is implemented as singleton i.e. both triggers prevent starting of job duplicate.
    /// Inspired by article https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-singletons?tabs=csharp .
    /// </summary>
    public class JobMicroserviceTriggerFunction
    {
        public const string InstanceId = "_InstanceId";
        private readonly ILogger<JobMicroserviceTriggerFunction> _logger;

        public JobMicroserviceTriggerFunction(ILogger<JobMicroserviceTriggerFunction> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This function is starter by timer.
        /// If an instance of the job is already running then it simply returns and do nothing.
        /// </summary>
        /// <param name="timerInfo"></param>
        /// <param name="orchestrationClient"></param>
        /// <returns></returns>
        [FunctionName(nameof(JobMicroserviceTimeTrigger))]
        public async Task JobMicroserviceTimeTrigger([TimerTrigger("%Schedule%")] TimerInfo timerInfo,
            [DurableClient] IDurableClient orchestrationClient)
        {
            var existingInstance = await orchestrationClient.GetStatusAsync(InstanceId);
            if (!existingInstance.IsAvailable())
            {
                _logger.LogInformation("Another instance of job has already been scheduled.");
                return;
            }

            _logger.LogInformation($"The job is executed by timer: {DateTime.Now}.");
            await orchestrationClient.StartNewAsync(nameof(JobMicroserviceFunction.JobMicroserviceOrchestrator), InstanceId, true);
        }

        /// <summary>
        /// Returns 309 (Conflict) http status code if an instance of the job already running.
        /// Returns specific type of response which contains endpoint to check job execution status.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="orchestrationClient"></param>
        /// <returns></returns>
        [FunctionName(nameof(ForceJobMicroservice))]
        public async Task<IActionResult> ForceJobMicroservice([HttpTrigger(AuthorizationLevel.Function, "post", Route = nameof(ForceJobMicroservice))] HttpRequest request,
            [DurableClient] IDurableClient orchestrationClient)
        {
            var existingInstance = await orchestrationClient.GetStatusAsync(InstanceId);
            if (!existingInstance.IsAvailable())
            {
                return new ConflictResult();
            }

            _logger.LogInformation("Forced execution of the job.");
            await orchestrationClient.StartNewAsync(nameof(JobMicroserviceFunction.JobMicroserviceOrchestrator), InstanceId, false);
            return orchestrationClient.CreateCheckStatusResponse(request, InstanceId);
        }
    }
}