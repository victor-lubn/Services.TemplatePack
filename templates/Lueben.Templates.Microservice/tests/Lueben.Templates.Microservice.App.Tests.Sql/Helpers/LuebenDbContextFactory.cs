using System;
using System.Diagnostics.CodeAnalysis;
using Lueben.Templates.Microservice.Data.Sql;
using Microsoft.EntityFrameworkCore;

namespace Lueben.Templates.Microservice.App.Tests.Sql.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class LuebenDbContextFactory
    {
        public static LuebenContext CreateInMemory(string databaseName = null)
        {
            var builder = new DbContextOptionsBuilder<LuebenContext>(new DbContextOptions<LuebenContext>());
            builder.UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString());

            return new LuebenContext(builder.Options);
        }
    }
}
