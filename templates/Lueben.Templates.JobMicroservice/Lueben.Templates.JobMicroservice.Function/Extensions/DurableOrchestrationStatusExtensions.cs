using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Lueben.Templates.JobMicroservice.Function.Extensions
{
    public static class DurableOrchestrationStatusExtensions
    {
        public static bool IsAvailable(this DurableOrchestrationStatus existingInstance)
        {
            return existingInstance == null
                || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Completed
                || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Failed
                || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Terminated;
        }
    }
}