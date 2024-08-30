Durable azure function application with example of chained activities.

Also added features:
Appinsight logging
Curcuit Breaker - to demonstrate how to check dependencies states for each activity.

Also shown an example how to handle exceptions raised by dependencies.

Example of triggering orchestration from postman

POST http://localhost:7071/runtime/webhooks/durabletask/orchestrators/ThereforeOrchestrator
BODY
{
    "applicationId": 1
}
Headers:
x-functions-key: <durable extension key as a value>.