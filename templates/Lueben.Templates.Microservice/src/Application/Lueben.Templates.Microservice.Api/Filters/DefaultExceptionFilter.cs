using System;
using System.Net;
using Lueben.Microservice.Api.Models;
using Lueben.Microservice.Api.PipelineFunction.Filters;
using Lueben.Templates.Microservice.Api.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lueben.Templates.Microservice.Api.Filters
{
    public class DefaultExceptionFilter : FunctionBaseExceptionFilter
    {
        public DefaultExceptionFilter(IHttpContextAccessor httpContextAccessor,
            ILogger<DefaultExceptionFilter> logger) : base(httpContextAccessor, logger)
        {
        }

        public override ErrorResult GetErrorResult(Exception exception)
        {
            var errorMessage = exception.ToString();
            return new ErrorResult(errorMessage, HttpStatusCode.InternalServerError, ErrorNames.InternalServerError);
        }
    }
}