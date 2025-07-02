using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLink.Core.Entities
{
    public class JobInterest
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        public Job Job { get; set; } = null!;

        public Guid DriverId { get; set; }
        public User Driver { get; set; } = null!;

        public string MobileNumber { get; set; } = null!;
        public DateTime RequestedAt { get; set; }

        public bool IsAccepted { get; set; } = false; // true = Poster has accepted this driver
    }
}
