using Microsoft.EntityFrameworkCore;
using TruckLink.Core.Entities;

namespace TruckLink.Infrastructure.Data
{
    public class TruckLinkDbContext : DbContext
    {
        public TruckLinkDbContext(DbContextOptions<TruckLinkDbContext> options) : base(options) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Company> Companies { get; set; } // ✅ fixed naming
        public DbSet<User> Users { get; set; }
        public DbSet<JobInterest> JobInterests { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoleMapping> UserRoles { get; set; }
        public DbSet<UserCompanyMapping> UserCompanies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // Start IDs from 10000
            // -------------------------

            modelBuilder.Entity<User>()
                .Property(x => x.Id)
                .UseIdentityColumn(10000, 1);

            modelBuilder.Entity<Job>()
                .Property(x => x.Id)
                .UseIdentityColumn(10000, 1);

            modelBuilder.Entity<JobInterest>()
                .Property(x => x.Id)
                .UseIdentityColumn(10000, 1);

            modelBuilder.Entity<Role>()
                .Property(x => x.Id)
                .UseIdentityColumn(10000, 1);

            modelBuilder.Entity<Company>()
                .Property(x => x.Id)
                .UseIdentityColumn(10000, 1);

            // -------------------------
            // User Config
            // -------------------------

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // -------------------------
            // Job Relationships
            // -------------------------

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

            // Company → Jobs (One-to-Many)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            // UserRole (Many-to-Many)
            // -------------------------

            modelBuilder.Entity<UserRoleMapping>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRoleMapping>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRoleMapping>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // -------------------------
            // UserCompany (Many-to-Many)
            // -------------------------

            modelBuilder.Entity<UserCompanyMapping>()
                .HasKey(x => new { x.UserId, x.CompanyId });

            modelBuilder.Entity<UserCompanyMapping>()
                .HasOne(x => x.User)
                .WithMany(u => u.UserCompanies)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserCompanyMapping>()
                .HasOne(x => x.Company)
                .WithMany(c => c.UserCompanies)
                .HasForeignKey(x => x.CompanyId);

            // -------------------------
            // OPTIONAL: Direct Company ↔ User (ONLY if needed)
            // -------------------------

            // ⚠️ Use this ONLY if User has CompanyId (single company system)
            // Otherwise REMOVE this block

            /*
            modelBuilder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            */

            // -------------------------
            // Company Field Config
            // -------------------------

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.Email)
                      .HasMaxLength(200);

                entity.Property(c => c.Phone)
                      .HasMaxLength(20);

                entity.Property(c => c.Address)
                      .HasMaxLength(500);
            });
        }
    }
}