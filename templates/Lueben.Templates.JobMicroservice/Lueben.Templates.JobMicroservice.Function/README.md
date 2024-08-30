Durable azure function application with example of chained activities.

Also added features:
Appinsight logging


Example of triggering job using "force" trigger.
Using this special trigger we can avoid starting multiple jobs at a time.

POST http://localhost:7071/runtime/webhooks/durabletask/orchestrators/ThereforeOrchestrator
BODY
{
    "applicationId": 1
}
Headers:
x-functions-key: function key.