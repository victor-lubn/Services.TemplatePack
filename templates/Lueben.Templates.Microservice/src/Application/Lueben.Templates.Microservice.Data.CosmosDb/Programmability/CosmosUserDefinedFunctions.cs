using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using DbConstantsCosmos = Lueben.Templates.Microservice.Data.CosmosDb.Constants.DbConstantsCosmos;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Programmability
{
    public class CosmosUserDefinedFunctions : CosmosProgrammabilityObjectBase
    {
        public override string FolderName => DbConstantsCosmos.UserDefinedFunctionsFolderName;

        protected override async Task Create(Container container, string name, string body)
        {
            var userDefinedFunctionProperty = new UserDefinedFunctionProperties
            {
                Id = name,
                Body = body
            };

            UserDefinedFunctionResponse userDefinedFunction = null;

            try
            {
                userDefinedFunction = await container.Scripts.ReadUserDefinedFunctionAsync(userDefinedFunctionProperty.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
            }

            if (userDefinedFunction != null)
            {
                await container.Scripts.ReplaceUserDefinedFunctionAsync(userDefinedFunctionProperty);
            }
            else
            {
                await container.Scripts.CreateUserDefinedFunctionAsync(userDefinedFunctionProperty);
            }
        }
    }
}
