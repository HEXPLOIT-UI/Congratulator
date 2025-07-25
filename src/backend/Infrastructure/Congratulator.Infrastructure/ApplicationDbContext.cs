using Congratulator.Domain.Birthdays.Entity;
using Congratulator.Domain.Users.Entity;
using Microsoft.EntityFrameworkCore;

namespace Congratulator.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<BirthdayEntity> Birthdays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var infrAssembly = typeof(ApplicationDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(infrAssembly);

        base.OnModelCreating(modelBuilder);
    }
}