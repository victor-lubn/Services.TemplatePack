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
#pragma warning disable SA1300 // Element should begin with upper-case letter
{
    public class ApplicationsFunctionSql : EntityFunction<Domain.Entities.Application, Application>
    {
        public ApplicationsFunctionSql(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor, AbstractValidator<Application> applicationModelValidator)
            : base(mediator, mapper, httpContextAccessor, applicationModelValidator)
        {
        }

        [FunctionName("getAllApplications")]
        [OpenApiOperation(operationId: "GetApplicationsByParametersSql", tags: new[] { "applications" }, Summary = "Retrieves application matching search criteria.", Description = "Retrieves application matching search criteria.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IList<Application>), Summary = "Application response", Description = "This returns the Application response")]
        public async Task<IActionResult> GetAllApplications([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/SqlApi/applications")] HttpRequest request)
        {
            return await GetAll<GetAllApplicationsQuerySql>();
        }

        [FunctionName("getApplication")]
        [OpenApiOperation(operationId: "GetApplicationSql", tags: new[] { "application" }, Summary = "Get application", Description = "get application.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(long))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Summary = "Application response", Description = "This returns the Application response")]
        [OpenApiNotFoundResponse]
        public async Task<IActionResult> GetApplication([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/SqlApi/application/{id}")] HttpRequest request, string id)
        {
            return await Get<GetApplicationQuerySql, string>(id);
        }

        [FunctionName("createApplication")]
        [OpenApiOperation(operationId: "AddApplicationSql", tags: new[] { "application" }, Summary = "Adds an application", Description = "This adds an application.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Application), Required = true, Description = "Application request model")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(long), Summary = "Application id", Description = "This returns the recently created application id")]
        public async Task<IActionResult> CreateApplication([HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/SqlApi/application")] HttpRequest request)
        {
            return await Create<CreateApplicationCommandSql, string>();
        }

        [FunctionName("patchApplication")]
        [OpenApiOperation(operationId: "PartialUpdateApplicationSql", tags: new[] { "application" }, Summary = "Partially updates an application", Description = "This updates an application partially.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(long))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Application), Required = true, Description = "Application request model")]
        [OpenApiNotFoundResponse]
        public async Task<IActionResult> PatchApplication([HttpTrigger(AuthorizationLevel.Function, "patch", Route = "v1/SqlApi/application/{id}")] HttpRequest request, string id)
        {
            return await Patch<PartiallyUpdateApplicationCommandSql, string>(id);
        }

        [FunctionName("putApplication")]
        [OpenApiOperation(operationId: "UpdateApplicationSql", tags: new[] { "application" }, Summary = "Updates an application", Description = "This updates an application.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(long))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Application), Required = true, Description = "Application request model")]
        [OpenApiNotFoundResponse]
        public async Task<IActionResult> PutApplication([HttpTrigger(AuthorizationLevel.Function, "put", Route = "v1/SqlApi/application/{id}")] HttpRequest request, string id)
        {
            return await Put<UpdateApplicationCommandSql, string>(id);
        }

        [FunctionName("deleteApplication")]
        [OpenApiOperation(operationId: "RemoveApplicationSql", tags: new[] { "application" }, Summary = "Deletes an application", Description = "This deletes an application.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(long))]
        [OpenApiNotFoundResponse]
        public async Task<IActionResult> DeleteApplication([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "v1/SqlApi/application/{id}")] HttpRequest request, string id)
        {
            return await Delete<DeleteApplicationCommandSql, string>(id);
        }

        protected override async Task<Application> DeserializeJsonBody()
        {
            return await GetValidatedRequest(true);
        }
    }
}
