using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Lueben.Templates.Microservice.Data.Sql
{
    public class LuebenContextFactory : IDesignTimeDbContextFactory<LuebenContext>
    {
        public LuebenContext CreateDbContext(string[] args)
        {
            var apiAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\Application\\Lueben.Templates.Microservice.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiAssemblyPath)
                .AddJsonFile("local.settings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<LuebenContext>();
            var connectionString = configuration.GetConnectionString("LuebenContext");

            builder.UseSqlServer(connectionString);
            return new LuebenContext(builder.Options);
        }
    }
}