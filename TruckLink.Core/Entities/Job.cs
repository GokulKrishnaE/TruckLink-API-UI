using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TruckLink.Core.Entities
{
    public class Job
    {
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string LoadItem { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public string ContactInfo { get; set; } = null!;

        [Required, MaxLength(100)]
        public string StartLocation { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Destination { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Earnings { get; set; }

        public double DistanceKm { get; set; }

        [MaxLength(500)]
        public string? MapUrl { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsCompleted { get; set; } = false;

        public Guid? AcceptedByDriverId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(AcceptedByDriverId))]
        public User? AcceptedByDriver { get; set; }

        public Guid CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedByUser { get; set; } = null!;

        public ICollection<JobInterest> Interests { get; set; } = new List<JobInterest>();
    }
}
