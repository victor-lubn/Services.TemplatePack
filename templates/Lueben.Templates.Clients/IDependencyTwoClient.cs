using System.Threading.Tasks;

namespace Lueben.Templates.Eventing.Clients.Stub
{
    public interface IDependencyTwoClient
    {
        Task<string> GetPIIDataForStepTwo(long appId);

        Task ExecuteStepTwo();
    }
}