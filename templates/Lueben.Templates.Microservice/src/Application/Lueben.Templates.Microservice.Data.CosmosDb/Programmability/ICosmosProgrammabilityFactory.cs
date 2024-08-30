using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Programmability
{
    public interface ICosmosProgrammabilityFactory
    {
        Task InitializeProgrammability(Container container);
    }
}