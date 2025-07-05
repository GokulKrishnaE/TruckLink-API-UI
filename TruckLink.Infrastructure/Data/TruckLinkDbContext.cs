using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TruckLink.Core.Entities;

namespace TruckLink.Infrastructure.Data
{
    public class TruckLinkDbContext : DbContext
    {
        public TruckLinkDbContext(DbContextOptions<TruckLinkDbContext> options) : base(options) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<JobInterest> JobInterests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Convert everything to snake_case if using PostgreSQL
            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                    foreach (var property in entity.GetProperties())
                        property.SetColumnName(ToSnakeCase(property.GetColumnName()));

                    foreach (var key in entity.GetKeys())
                        key.SetName(ToSnakeCase(key.GetName()));

                    foreach (var fk in entity.GetForeignKeys())
                        fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()));

                    foreach (var index in entity.GetIndexes())
                        index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
                }
            }

            // Custom Fluent Configurations
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

        // Convert PascalCase to snake_case
        private static string ToSnakeCase(string input)
        {
            return string.Concat(input.Select((ch, i) =>
                i > 0 && char.IsUpper(ch) ? "_" + ch : ch.ToString())).ToLower();
        }
    }
}
