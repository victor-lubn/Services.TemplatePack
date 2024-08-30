using System.Threading.Tasks;
using Lueben.Templates.ApiClient.Models;

namespace Lueben.Templates.ApiClient
{
    public interface IApiClient
    {
        Task<int> AddAsync(Entity entity);
    }
}