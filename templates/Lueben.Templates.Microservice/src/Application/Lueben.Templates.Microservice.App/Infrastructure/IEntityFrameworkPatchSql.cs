using System.Threading.Tasks;

namespace Lueben.Templates.Microservice.App.Infrastructure
{
    public interface IEntityFrameworkPatchSql
    {
        Task ApplyPatch<TEntity, TDto>(TDto dto)
            where TEntity : class;
    }
}
