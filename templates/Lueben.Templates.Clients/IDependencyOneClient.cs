using System.Collections.Generic;
using System.Threading.Tasks;
using Lueben.Templates.Eventing.Clients.Stub.Models;

namespace Lueben.Templates.Eventing.Clients.Stub
{
    public interface IDependencyOneClient 
    {
        IEnumerable<Application> GetAllApplications();

        Task<int> GetPIIDataForStepOne(long appId);

        Task ExecuteStepOne();
    }
}