using System.Collections.Generic;

namespace Lueben.Templates.JobMicroservice.Function.Logging
{
    public static class LogConstants
    {
        public const string ApplicationIdKey = "ApplicationId";

        public const string RetryCountKey = "RetryCount";

        public static readonly List<string> AllProperties = new List<string> { ApplicationIdKey, RetryCountKey };
    }
}
