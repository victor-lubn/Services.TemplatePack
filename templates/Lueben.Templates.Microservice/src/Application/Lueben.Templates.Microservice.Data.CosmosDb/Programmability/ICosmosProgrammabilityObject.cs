using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Programmability
{
    public interface ICosmosProgrammabilityObject
    {
        Task Initialize(Container container);
    }
}
