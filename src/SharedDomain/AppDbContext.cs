using Microsoft.EntityFrameworkCore;
using SharedDomain.Entities;

namespace SharedDomain
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<PersonEntity> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonEntity>();

            base.OnModelCreating(modelBuilder);
        }
    }
}