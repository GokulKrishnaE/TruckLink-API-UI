namespace TruckLink.API.DTOs
{
    public class JobDto
    {
        public int Id { get; set;}
        public string LoadItem { get; set; } = null!;
        public string StartLocation { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public decimal Earnings { get; set; }
        public double DistanceKm { get; set; }
        public string? MapUrl { get; set; }

        public bool IsCompleted { get; set; }
    }
}
