namespace TruckLink.API.DTOs
{
    public class JobDetailsDto
    {
        public Guid Id { get; set; }
        public string LoadItem { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ContactInfo { get; set; } = null!;
        public string StartLocation { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public decimal Earnings { get; set; }
        public double DistanceKm { get; set; }
        public string? MapUrl { get; set; }
        public bool IsAccepted { get; set; }

        public bool IsCompleted { get; set; }
        public Guid? AcceptedByDriverId { get; set; }
        public DateTime CreatedAt { get; set; }

        // List of driver requests (interests)
        public List<JobInterestDto> Interests { get; set; } = new();
    }
}
