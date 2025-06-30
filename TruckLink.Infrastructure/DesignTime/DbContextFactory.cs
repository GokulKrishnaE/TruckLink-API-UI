using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TruckLink.Infrastructure.Data
{
    public class TruckLinkDbContextFactory : IDesignTimeDbContextFactory<TruckLinkDbContext>
    {
        public TruckLinkDbContext CreateDbContext(string[] args)
        {
            // IMPORTANT: Set base path to the API project folder or where appsettings.json actually is
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\TruckLink.API");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TruckLinkDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new TruckLinkDbContext(optionsBuilder.Options);
        }
    }
}
