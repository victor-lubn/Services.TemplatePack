using System.Collections.Generic;

namespace Lueben.Templates.Microservice.Functions.Logging
{
    public static class LogConstants
    {
        public const string ApplicationIdKey = "ApplicationId";

        public const string ApplicationStatusKey = "Application status";

        public const string PartyIdKey = "PartyId";

        public static readonly List<string> AllProperties = new List<string> { ApplicationIdKey, PartyIdKey, ApplicationStatusKey };
    }
}
