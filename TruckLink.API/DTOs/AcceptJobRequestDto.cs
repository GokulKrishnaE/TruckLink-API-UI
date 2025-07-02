namespace TruckLink.API.DTOs
{
    public class AcceptJobRequestDto
    {
        public Guid JobId { get; set; }
        public Guid DriverId { get; set; }
    }
}
