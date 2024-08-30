using System;
using System.ComponentModel;

namespace Lueben.Templates.Microservice.Api.Contract.Models
{
    public class Application
    {
        [ReadOnly(true)]
        public string ApplicationId { get; set; }

        public string PreferredDepotId { get; set; }

        public string Comments { get; set; }

        public string RejectionReason { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }
    }
}
