namespace Lueben.Templates.Microservice.App.UseCases.Applications
{
    public class ApplicationCommandBase : TrackingCommandBase
    {
        public string PreferredDepotId { get; set; }

        public string Comments { get; set; }

        public string RejectionReason { get; set; }
    }
}
