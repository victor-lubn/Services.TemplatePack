using System.Threading;
using System.Threading.Tasks;
using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lueben.Templates.Microservice.App.Infrastructure
{
    public interface ILuebenContextSql
    {
        DbSet<Application> Application { get; set; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        void CleanTrackedEntity<T>(T entity)
            where T : class;

        EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
            where TEntity : class;

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
            where TEntity : class;
    }
}
