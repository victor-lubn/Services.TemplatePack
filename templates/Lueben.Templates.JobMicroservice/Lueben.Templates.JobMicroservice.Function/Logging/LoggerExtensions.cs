using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Lueben.Templates.JobMicroservice.Function.Logging
{
    public static class LoggerExtensions
    {
        public static IDisposable BeginApplicationScope(this ILogger logger, long applicationId)
        {
            return logger?.BeginScope(new Dictionary<string, object>
            {
                [LogConstants.ApplicationIdKey] = applicationId
            });
        }
    }
}