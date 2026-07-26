using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLink.Core.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public ICollection<UserCompanyMapping> UserCompanies { get; set; } = new List<UserCompanyMapping>();
    }
}
