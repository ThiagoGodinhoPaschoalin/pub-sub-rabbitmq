using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace SharedDomain
{
    public static class DiServices
    {
        public static void AddSharedDomain(this IServiceCollection services) 
        {
            services.AddDbContextPool<AppDbContext>(opt =>
            {
                opt.UseInMemoryDatabase("dbInMemory");
                opt.EnableSensitiveDataLogging();
            });
        }
    }

    /// <summary>
    /// To enable use scaffolded with DbContext in another library
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbInMemory");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
