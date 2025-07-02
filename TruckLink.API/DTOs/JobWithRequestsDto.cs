namespace TruckLink.API.DTOs
{
    public class JobWithRequestsDto
    {
        public Guid JobId { get; set; }
        public string LoadItem { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ContactInfo { get; set; } = null!;
        public string StartLocation { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public decimal Earnings { get; set; }
        public double DistanceKm { get; set; }
        public bool IsAccepted { get; set; }

        public bool IsCompleted { get; set; }
        public List<JobInterestDto> Interests { get; set; } = new();
    }
}
