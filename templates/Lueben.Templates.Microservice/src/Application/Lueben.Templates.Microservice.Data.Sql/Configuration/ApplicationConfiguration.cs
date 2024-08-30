using Lueben.Templates.Microservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lueben.Templates.Microservice.Data.Sql.Configuration
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.HasKey(e => e.Id)
                .HasName("PK__Application");

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Comments)
                .HasMaxLength(1000)
                .IsUnicode(false);

            builder.Property(e => e.Created).HasColumnType("datetime")
                .HasDefaultValueSql("getutcdate()");

            builder.Property(e => e.PreferredDepotId)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.RejectionReason)
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(e => e.Updated).HasColumnType("datetime")
                .HasDefaultValueSql("getutcdate()");
        }
    }
}
