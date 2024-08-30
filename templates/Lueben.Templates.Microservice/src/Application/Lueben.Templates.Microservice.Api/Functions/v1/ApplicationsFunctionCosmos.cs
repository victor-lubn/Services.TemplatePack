using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Lueben.Microservice.EntityFunction;
using Lueben.Microservice.Mediator;
using Lueben.Microservice.OpenApi.Attributes;
using Lueben.Templates.Microservice.Api.Contract.Models;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.CreateApplication;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.DeleteApplication;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication;
using Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetAllApplications;
using Lueben.Templates.Microservice.App.UseCases.Applications.Queries.GetApplication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace Lueben.Templates.Microservice.Api.Functions.v1
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    public class ApplicationsFunctionCosmos : EntityFunction<Domain.Entities.Application, Application>
    {
        public ApplicationsFunctionCosmos(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor, AbstractValidator<Application> applicationModelValidator)
            : base(mediator, mapper, httpContextAccessor, applicationModelValidator)
        {
        }

        [FunctionName("getAllApplicationsCosmos")]
        [OpenApiOperation(operationId: "GetApplicationsByParameters", tags: new[] { "applications" }, Summary = "Retrieves application matching search criteria.", Description = "Retrieves application matching search criteria.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IList<Application>), Summary = "Application response", Description = "This returns the Application response")]
        public async Task<IActionResult> GetAllApplications([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/CosmosApi/applications")] HttpRequest request)
        {
            return await GetAll<GetAllApplicationsQueryCosmos>();
        }

        [FunctionName("getApplicationCosmos")]
        [OpenApiOperation(operationId: "GetApplication", tags: new[] { "application" }, Summary = "Get application", Description = "get application.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(long))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Summary = "Application response", Description = "This returns the Application response")]
        [OpenApiNotFoundResponse]
        public async Task<IActionResult> GetApplication([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/CosmosApi/application/{id}")] HttpRequest request, string id)
        {
            return await Get<GetApplicationQueryCosmos, string>(id);
        }

        [FunctionName("createApplicationCosmos")]
        [OpenApiOperation(operationId: "AddApplication", tags: new[] { "application" }, Summary = "Adds an application", Description = "This adds an application.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Application), Required = true, Description = "Application request model")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(long), Summary = "Application id", Description = "This returns the recently created application id")]
        public async Task<IActionResult> CreateApplication([HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/CosmosApi/application")] HttpRequest request)
        {
            return await Create<CreateApplicationCommandCosmos, string>();
        }

        [FunctionName("patchApplicationCosmos")]
        [OpenApiOperation(operationId: "PartialUpdateApplication", tags: new[] { "application" }, Summary = "Partially updates an application", Description = "This updates an application partially.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(long))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Application), Required = true, Description = "Application request model")]
        [OpenApiNotFoundResponse]
        public async Task<IActionResult> PatchApplication([HttpTrigger(AuthorizationLevel.Function, "patch", Route = "v1/CosmosApi/application/{id}")] HttpRequest request, string id)
        {
            return await Patch<PartiallyUpdateApplicationCommandCosmos, string>(id);
        }

        [FunctionName("putApplicationCosmos")]
        [OpenApiOperation(operationId: "UpdateApplication", tags: new[] { "application" }, Summary = "Updates an application", Description = "This updates an application.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(long))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Application), Required = true, Description = "Application request model")]
        [OpenApiNotFoundResponse]
        public async Task<IActionResult> PutApplication([HttpTrigger(AuthorizationLevel.Function, "put", Route = "v1/CosmosApi/application/{id}")] HttpRequest request, string id)
        {
            return await Put<UpdateApplicationCommandCosmos, string>(id);
        }

        [FunctionName("deleteApplicationCosmos")]
        [OpenApiOperation(operationId: "RemoveApplication", tags: new[] { "application" }, Summary = "Deletes an application", Description = "This deletes an application.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(long))]
        [OpenApiNotFoundResponse]
        public async Task<IActionResult> DeleteApplication([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "v1/CosmosApi/application/{id}")] HttpRequest request, string id)
        {
            return await Delete<DeleteApplicationCommandCosmos, string>(id);
        }

        protected override async Task<Application> DeserializeJsonBody()
        {
            return await GetValidatedRequest(true);
        }
    }
}
