namespace Lueben.Templates.Microservice.Domain.Entities
{
    public class Application : Entity
    {
        public string PreferredDepotId { get; set; }

        public string Comments { get; set; }

        public string RejectionReason { get; set; }
    }
}
