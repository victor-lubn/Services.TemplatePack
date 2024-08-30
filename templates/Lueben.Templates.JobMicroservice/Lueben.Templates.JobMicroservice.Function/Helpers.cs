using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Newtonsoft.Json;

namespace Lueben.Templates.JobMicroservice.Function
{
    public static class Helpers
    {
        public static async Task CreateWorkloadJsonBlob<T>(BlobContainerClient blobContainerClient, T workload, int i, string prefix)
        {
            await blobContainerClient.CreateIfNotExistsAsync();
            var blobClient = blobContainerClient.GetBlockBlobClient($"{prefix}{i}.json");
            var jsonContent = JsonConvert.SerializeObject(workload, Formatting.None);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            await blobClient.UploadAsync(stream);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> list, int size)
        {
            for (var i = 0; i < list.Count; i += size)
            {
                yield return list.GetRange(i, Math.Min(size, list.Count - i));
            }
        }
    }
}
