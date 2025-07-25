using Congratulator.Domain.Birthdays.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Congratulator.Infrastructure.Configurations.Data;

public class BirthdayEntityConfiguration : BaseEntityConfiguration<BirthdayEntity>, IEntityTypeConfiguration<BirthdayEntity>
{
    public override void Configure(EntityTypeBuilder<BirthdayEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("birthdays");

        builder.Property(h => h.FirstName)
                .IsRequired()
               .HasColumnName("first_name");

        builder.Property(h => h.LastName)
                .IsRequired()
               .HasColumnName("last_name");

        builder.Property(h => h.LastName)
                .IsRequired()
               .HasColumnName("last_name");

        builder.Property(l => l.DateOfBirth)
               .HasColumnName("date_of_birth")
               .HasColumnType("timestamp with time zone");

        builder.Property(l => l.PhotoPath)
               .HasColumnName("photo_path");

        builder.Property(l => l.IsActive)
               .HasColumnName("is_active")
               .HasDefaultValue(true);

        builder.HasOne(l => l.User)
               .WithMany(u => u.Birthdays)
               .HasForeignKey(l => l.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_birthdays_user");
    }
}