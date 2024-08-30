using System.Linq;
using System.Reflection;
using Lueben.Templates.Microservice.App.Infrastructure;
using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lueben.Templates.Microservice.Data.Sql
{
    public class LuebenContext : DbContext, ILuebenContextSql
    {
        public LuebenContext()
        {
        }

        public LuebenContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Application> Application { get; set; }

        public void CleanTrackedEntity<T>(T entity)
            where T : class
        {
            var trackedEntity = ChangeTracker.Entries<T>().FirstOrDefault(x => x.Entity == entity);
            if (trackedEntity != null)
            {
                trackedEntity.State = EntityState.Detached;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}
