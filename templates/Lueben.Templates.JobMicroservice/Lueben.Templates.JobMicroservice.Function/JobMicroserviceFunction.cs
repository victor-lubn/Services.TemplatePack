using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Lueben.Microservice.DurableFunction;
using Lueben.Microservice.DurableFunction.Extensions;
using Lueben.Microservice.EventHub;
using Lueben.Templates.Eventing.Clients.Stub;
using Lueben.Templates.Eventing.Clients.Stub.Models;
using Lueben.Templates.JobMicroservice.Function.Logging;
using Lueben.Templates.JobMicroservice.Function.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Lueben.Templates.JobMicroservice.Function
{
    /// <summary>
    /// Example of how using durable functions we can split one long job
    /// to a number of quick activities
    /// to about timeouts happened in functions running on consumption plan.
    /// timeout for a function is 5 minutes by default.
    /// </summary>
    public class JobMicroserviceFunction
    {
        // maximum is needed to not exceed event hub batch limit.
        private const int MaximumApplicationsInBatch = 100;
        private const string ContainerName = "jobmicroservice-workitems";
        private const string ApplicationsFilePrefix = "applications-";
        private const string EmailsFilePrefix = "emails-";

        private const string EmailBatchFile = ContainerName + "/" + EmailsFilePrefix;
        private const string ApplicationBatchFile = ContainerName + "/" + ApplicationsFilePrefix;

        private readonly ILogger<JobMicroserviceFunction> _logger;
        private readonly IOptionsSnapshot<JobOptions> _options;
        private readonly IOptionsSnapshot<WorkflowOptions> _workflowOptions;
        private readonly IEventDataSender _eventDataSender;
        private readonly IDependencyOneClient _dependencyOneClient;
        private readonly TelemetryClient _telemetryClient;

        public JobMicroserviceFunction(ILogger<JobMicroserviceFunction> logger,
            TelemetryConfiguration telemetryConfiguration,
            IOptionsSnapshot<JobOptions> options,
            IOptionsSnapshot<WorkflowOptions> workflowOptions,
            IEventDataSender eventDataSender,
            IDependencyOneClient dependencyOneClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _workflowOptions = workflowOptions ?? throw new ArgumentNullException(nameof(workflowOptions));
            _eventDataSender = eventDataSender ?? throw new ArgumentNullException(nameof(eventDataSender));
            _dependencyOneClient = dependencyOneClient ?? throw new ArgumentNullException(nameof(dependencyOneClient));
            _telemetryClient = new TelemetryClient(telemetryConfiguration ?? throw new ArgumentNullException(nameof(telemetryConfiguration)));
        }

        /// <summary>
        /// Orchestrator function.
        /// If needed here we can check that feature toggle is enabled for this job.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [FunctionName(nameof(JobMicroserviceOrchestrator))]
        public async Task JobMicroserviceOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            _telemetryClient.TrackUsingCorrelationTraceContext();
            var logger = context.CreateReplaySafeLogger(_logger);

            // do not enable retry logic if job execution was triggered manually to propagate failure
            var retryOperation = context.GetInput<bool>();
            var retryOptions = new RetryOptions(_workflowOptions.Value.ActivityRetryIntervalTime,
                retryOperation ? _workflowOptions.Value.MaxEventRetryCount : 1);

            if (!context.IsReplaying)
            {
                logger.LogInformation($"The job executed at: {context.CurrentUtcDateTime}");
            }

            try
            {
                var chunksCount = await context.CallActivityWithRetryAsync<int>(nameof(JobMicroserviceCreateApplicationBatches), retryOptions, null);

                var emailsTasks = new List<Task<int>>();
                for (var i = 0; i < chunksCount; i++)
                {
                    var emailsExists = await context.CallActivityWithRetryAsync<bool>(nameof(JobMicroserviceProcessApplicationBatch), retryOptions, i);
                    if (emailsExists)
                    {
                        var emailTask = context.CallActivityWithRetryAsync<int>(nameof(JobMicroserviceProcessEmailBatch), retryOptions, i);
                        emailsTasks.Add(emailTask);
                    }
                }

                await Task.WhenAll(emailsTasks);
                try
                {
                    var sentEmails = emailsTasks.Sum(t => t.Result);
                    logger.LogInformation($"Sent {sentEmails} emails.");
                }
                catch (AggregateException e)
                {
                    logger.LogError(e.GetBaseException(), $"Failed to send one of the email.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while job execution.");
                throw;
            }
        }

        /// <summary>
        /// This activity gets data for processing
        /// and split it into chunks stored in separate blobs
        /// for processing by another activity.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="container">BlobContainer initialized by blob input binding.</param>
        /// <returns></returns>
        [FunctionName(nameof(JobMicroserviceCreateApplicationBatches))]
        public async Task<int> JobMicroserviceCreateApplicationBatches([ActivityTrigger] object input,
            [Blob(ContainerName)] BlobContainerClient container)
        {
            var data = GetApplications().ToList();
            _logger.LogInformation($"Found {data.Count} applications.");
            var chunks = Helpers.SplitList(data, Math.Min(_options.Value.BatchCount, MaximumApplicationsInBatch))
                .ToArray();
            /* remove not processed workload from previous failed run */
            await container.DeleteIfExistsAsync();
            await container.CreateAsync();

            try
            {
                for (var i = 0; i < chunks.Length; i++)
                {
                    await Helpers.CreateWorkloadJsonBlob(container, chunks[i], i, ApplicationsFilePrefix);
                }
            }
            catch (Exception)
            {
                await container.DeleteAsync();
                throw;
            }

            _logger.LogInformation($"Created {chunks.Length} application files.");
            return chunks.Length;
        }

        /// <summary>
        /// Process one single chunk of applications and prepares workload for another activity.
        /// The workload stored as a blob.
        /// </summary>
        /// <param name="batch">number of the workload to process.</param>
        /// <param name="blobClient">workload initialized from blob input/output binding by name and workload id (batch).
        /// After successful processing the blob is deleted.
        /// </param>
        /// <param name="emailsStream"></param>
        /// <returns></returns>
        [FunctionName(nameof(JobMicroserviceProcessApplicationBatch))]
        public async Task<bool> JobMicroserviceProcessApplicationBatch([ActivityTrigger] int batch,
            [Blob(ApplicationBatchFile + "{batch}.json", FileAccess.ReadWrite)] BlobClient blobClient,
            [Blob(EmailBatchFile + "{batch}.json", FileAccess.Write)] Stream emailsStream)
        {
            List<EngagementApplication> applications;
            try
            {
                BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();
                var json = downloadResult.Content.ToString();
                applications = JsonConvert.DeserializeObject<List<EngagementApplication>>(json);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to read application workload {ApplicationBatchFile}{batch}.json.");
                return false;
            }

            var events = new List<Event<EmailData>>();
            if (applications != null)
            {
                foreach (var application in applications)
                {
                    events.AddRange(GetApplicationEmails(application));
                }
            }

            if (events.Count > 0)
            {
                var json = JsonConvert.SerializeObject(events);
                await using var stream = Helpers.GenerateStreamFromString(json);
                await using var sw = new StreamWriter(emailsStream);
                await sw.WriteLineAsync(json);
            }

            await blobClient.DeleteAsync();

            return events.Count > 0;
        }

        /// <summary>
        /// Sending emails using batch send method which is sources from data in the blob file.
        /// </summary>
        /// <param name="batch">number of the workload to process.</param>
        /// <param name="blobClient">
        /// After successful processing the blob is deleted.</param>
        /// <returns></returns>
        [FunctionName(nameof(JobMicroserviceProcessEmailBatch))]
        public async Task<int> JobMicroserviceProcessEmailBatch([ActivityTrigger] int batch,
            [Blob(EmailBatchFile + "{batch}.json", FileAccess.ReadWrite)] BlobClient blobClient)
        {
            List<Event<EmailData>> emails;
            try
            {
                BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();
                var json = downloadResult.Content.ToString();
                if (string.IsNullOrEmpty(json))
                {
                    return 0;
                }

                emails = JsonConvert.DeserializeObject<List<Event<EmailData>>>(json);
                if (emails == null)
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to read workload {EmailBatchFile}{batch}.json.");
                return 0;
            }

            await _eventDataSender.SendEventsListAsync(emails);
            await blobClient.DeleteAsync();
            return emails.Count;
        }

        public IEnumerable<EngagementApplication> GetApplications()
        {
            var allApps = _dependencyOneClient.GetAllApplications();
            return allApps.Select(a => new EngagementApplication
            {
                Id = a.Id,
                Parties = a.Parties?.Select(p => new EngagementParty
                {
                    Id = p.Id
                }).ToArray()
            });
        }

        public List<Event<EmailData>> GetApplicationEmails(EngagementApplication application)
        {
            var eventsList = new List<Event<EmailData>>();
            using (_logger.BeginApplicationScope(application.Id))
            {
                _logger.LogInformation($"Start processing application with id {application.Id}");

                if (application.Parties == null)
                {
                    return eventsList;
                }

                foreach (var party in application.Parties)
                {
                    var emailData = new EmailData
                    {
                        ApplicationId = application.Id,
                        PartyId = party.Id,
                    };

                    eventsList.Add(new Event<EmailData>
                    {
                        Type = "email",
                        Data = emailData
                    });
                }
            }

            return eventsList;
        }
    }
}