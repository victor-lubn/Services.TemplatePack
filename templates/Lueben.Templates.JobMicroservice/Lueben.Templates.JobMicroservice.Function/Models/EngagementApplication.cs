namespace Lueben.Templates.JobMicroservice.Function.Models
{
    public class EngagementApplication
    {
        public long Id { get; set; }

        public EngagementParty[] Parties { get; set; }
    }
}