namespace TruckLink.API.DTOs
{
    public class JobInterestDto
    {
        public Guid DriverId { get; set; }
        public string DriverName { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public DateTime RequestedAt { get; set; }
    }
}
