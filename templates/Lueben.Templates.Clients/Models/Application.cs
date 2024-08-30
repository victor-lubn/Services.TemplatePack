using System.Collections.Generic;

namespace Lueben.Templates.Eventing.Clients.Stub.Models
{
    public class Application
    {
        public int Id { get; set; }

        public IList<Party> Parties { get; set; }
    }

    public class Party
    {
        public int Id { get; set; }

        public string PiiData { get; set; }
    }
}