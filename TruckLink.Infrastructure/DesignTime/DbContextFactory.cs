using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TruckLink.Infrastructure.Data
{
    public class TruckLinkDbContextFactory : IDesignTimeDbContextFactory<TruckLinkDbContext>
    {
        public TruckLinkDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\TruckLink.API");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var environment = config["ASPNETCORE_ENVIRONMENT"] ?? "Development";
            var connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<TruckLinkDbContext>();

            if (environment == "Development")
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(connectionString); // PostgreSQL for prod
            }

            return new TruckLinkDbContext(optionsBuilder.Options);
        }
    }
}
