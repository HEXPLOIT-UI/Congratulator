using Congratulator.Domain.Users.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Congratulator.Infrastructure.Configurations.Data;

public class UserEntityConfiguration : BaseEntityConfiguration<UserEntity>, IEntityTypeConfiguration<UserEntity>
{
    public override void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("users");

        builder.Property(u => u.Login)
               .IsRequired()
               .HasColumnName("login");

        builder.HasIndex(u => u.Login)
                .IsUnique();

        builder.Property(u => u.FirstName)
               .IsRequired()
               .HasColumnName("first_name");

        builder.Property(u => u.LastName)
               .IsRequired()
               .HasColumnName("last_name");

        builder.Property(u => u.Role)
               .IsRequired()
               .HasColumnName("role")
               .HasDefaultValue("User");

        builder.Property(u => u.PasswordHash)
               .IsRequired()
               .HasColumnName("password_hash");

        builder.HasMany(u => u.Birthdays)
               .WithOne(l => l.User)
               .HasForeignKey(l => l.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(DataSeedHelper.GetUserEntities());
    }
}
