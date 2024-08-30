using System.Collections.Generic;
using System.Threading.Tasks;
using Lueben.Templates.Eventing.Clients.Stub.Models;

namespace Lueben.Templates.Eventing.Clients.Stub.Stubs
{
    public class JobDependencyClientStub : IDependencyOneClient
    {
        public IEnumerable<Application> GetAllApplications()
        {
            for (var i = 0; i < 1000; i++)
            {
                var party = new Party {Id = i, PiiData = "pii"};
                yield return new Application { Id = i, Parties = new List<Party> { party }};
            }
        }

        public Task<int> GetPIIDataForStepOne(long appId)
        {
            throw new System.NotImplementedException();
        }

        public Task ExecuteStepOne()
        {
            throw new System.NotImplementedException();
        }
    }
}