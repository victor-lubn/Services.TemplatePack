using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Programmability
{
    public class CosmosProgrammabilityFactory : ICosmosProgrammabilityFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CosmosProgrammabilityFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task InitializeProgrammability(Container container)
        {
            var programmabilityObjects = _serviceProvider.GetServices<ICosmosProgrammabilityObject>();
            foreach (var programmabilityObject in programmabilityObjects)
            {
                await programmabilityObject.Initialize(container);
            }
        }
    }
}