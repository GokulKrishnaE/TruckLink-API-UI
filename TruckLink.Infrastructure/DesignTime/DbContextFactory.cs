using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.IO;

namespace TruckLink.Infrastructure.Data
{
    public class TruckLinkDbContextFactory : IDesignTimeDbContextFactory<TruckLinkDbContext>
    {
        public TruckLinkDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\TruckLink.API");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<TruckLinkDbContext>();

            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 2, 0)));

            return new TruckLinkDbContext(optionsBuilder.Options);
        }
    }
}
