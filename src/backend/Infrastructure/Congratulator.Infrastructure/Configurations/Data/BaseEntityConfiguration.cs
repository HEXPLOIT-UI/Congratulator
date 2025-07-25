using Congratulator.Domain.Base.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Congratulator.Infrastructure.Configurations.Data;

public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.EntityId);

        builder.Property(e => e.EntityId)
               .HasColumnName("entity_id")
               .HasColumnType("uuid")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .HasColumnType("timestamp with time zone")
               .HasDefaultValueSql("now()");
    }
}