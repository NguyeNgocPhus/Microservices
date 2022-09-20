using Identity.Core.Events;
using Identity.Core.ReadModels;
using Identity.Infrastructure.Databases.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Databases;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
            
    }
    public DbSet<UserReadModel> Users { get; set; }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }

}