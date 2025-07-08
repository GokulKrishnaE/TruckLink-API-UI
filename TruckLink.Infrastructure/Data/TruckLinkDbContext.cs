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

            //modelBuilder.Entity<User>().ToTable("users","public"); ;
            //modelBuilder.Entity<Job>().ToTable("jobs","public"); ;
            //modelBuilder.Entity<JobInterest>().ToTable("job_interests","public"); ;

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
    }
}
