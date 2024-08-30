using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using DbConstantsCosmos = Lueben.Templates.Microservice.Data.CosmosDb.Constants.DbConstantsCosmos;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Programmability
{
    public class CosmosStoredProcedures : CosmosProgrammabilityObjectBase
    {
        public override string FolderName => DbConstantsCosmos.StoredProceduresFolderName;

        protected override async Task Create(Container container, string name, string body)
        {
            var storedProcedureProperties = new StoredProcedureProperties
            {
                Id = name,
                Body = body
            };

            StoredProcedureResponse storedProcedure = null;

            try
            {
                storedProcedure = await container.Scripts.ReadStoredProcedureAsync(storedProcedureProperties.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
            }

            if (storedProcedure != null)
            {
                await container.Scripts.ReplaceStoredProcedureAsync(storedProcedureProperties);
            }
            else
            {
                await container.Scripts.CreateStoredProcedureAsync(storedProcedureProperties);
            }
        }
    }
}
