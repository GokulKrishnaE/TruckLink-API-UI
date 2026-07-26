using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLink.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        // Only for Drivers
        public ICollection<Job> PostedJobs { get; set; } = new List<Job>();
        public ICollection<Job> AcceptedJobs { get; set; } = new List<Job>();

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<UserRoleMapping> UserRoles { get; set; } = new List<UserRoleMapping>();
        public ICollection<UserCompanyMapping> UserCompanies { get; set; } = new List<UserCompanyMapping>();
    }
}