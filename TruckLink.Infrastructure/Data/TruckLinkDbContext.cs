using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TruckLink.Core.Entities;

namespace TruckLink.Infrastructure.Data
{
    public class TruckLinkDbContext : DbContext
    {
        public TruckLinkDbContext(DbContextOptions<TruckLinkDbContext> options): base(options) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<JobInterest> JobInterests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply snake_case naming if using PostgreSQL
            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                    foreach (var property in entity.GetProperties())
                    {
                        property.SetColumnName(ToSnakeCase(property.Name));
                    }

                    foreach (var key in entity.GetKeys())
                    {
                        key.SetName(ToSnakeCase(key.GetName()));
                    }

                    foreach (var fk in entity.GetForeignKeys())
                    {
                        fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()));
                    }

                    foreach (var index in entity.GetIndexes())
                    {
                        index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
                    }
                }
            }

            // Your existing config
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Job>()
                .HasOne(j => j.AcceptedByDriver)
                .WithMany(u => u.AcceptedJobs)
                .HasForeignKey(j => j.AcceptedByDriverId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.CreatedByUser)
                .WithMany(u => u.PostedJobs)
                .HasForeignKey(j => j.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // Helper to convert to snake_case
        private static string ToSnakeCase(string name)
        {
            return string.Concat(
                name.Select((x, i) => i > 0 && char.IsUpper(x)
                    ? "_" + x
                    : x.ToString()))
                .ToLower();
        }
    }


}

