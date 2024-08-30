using System;
using System.Net;
using Lueben.Microservice.Api.Models;
using Lueben.Microservice.Api.PipelineFunction.Filters;
using Lueben.Microservice.Api.ValidationFunction.Exceptions;
using Lueben.Templates.Microservice.Api.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lueben.Templates.Microservice.Api.Filters
{
    public class ModelValidationExceptionFilter : FunctionBaseExceptionFilter
    {
        public ModelValidationExceptionFilter(IHttpContextAccessor httpContextAccessor,
            ILogger<DefaultExceptionFilter> logger) : base(httpContextAccessor, logger)
        {
        }

        public override ErrorResult GetErrorResult(Exception exception)
        {
            return exception switch
            {
                ModelNotValidException ex => new ErrorResult(ex.Message, HttpStatusCode.BadRequest, ErrorNames.ModelNotValid, ex.ValidationErrors),
                _ => null
            };
        }
    }
}