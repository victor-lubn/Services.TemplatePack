using System;
using System.Net;
using Lueben.Microservice.Api.Models;
using Lueben.Microservice.Api.PipelineFunction.Filters;
using Lueben.Templates.Microservice.Api.Constants;
using Lueben.Templates.Microservice.App.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lueben.Templates.Microservice.Api.Filters
{
    public class EntityExceptionFilter : FunctionBaseExceptionFilter
    {
        public EntityExceptionFilter(IHttpContextAccessor httpContextAccessor,
            ILogger<DefaultExceptionFilter> logger) : base(httpContextAccessor, logger)
        {
        }

        public override ErrorResult GetErrorResult(Exception exception)
        {
            return exception switch
            {
                EntityNotFoundException _ => new ErrorResult(exception.Message, HttpStatusCode.NotFound, ErrorNames.EntityNotFound),
                _ => null,
            };
        }
    }
}