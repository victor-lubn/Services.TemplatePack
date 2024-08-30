using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Programmability
{
    public abstract class CosmosProgrammabilityObjectBase : ICosmosProgrammabilityObject
    {
        public abstract string FolderName { get; }

        public async Task Initialize(Container container)
        {
            var dllDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var currentDirectory = Path.GetFullPath(Path.Combine(dllDirectoryPath, @"..\\")) + FolderName;
            foreach (var filePath in Directory.EnumerateFiles(currentDirectory, "*"))
            {
                var body = await File.ReadAllTextAsync(filePath);
                var name = Path.GetFileNameWithoutExtension(filePath);
                await Create(container, name, body);
            }
        }

        protected abstract Task Create(Container container, string name, string body);
    }
}
