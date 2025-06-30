namespace TruckLink.API.DTOs
{
    public class InterestedJobDto
    {
        public JobDto Job { get; set; } = null!;
        public bool IsAcceptedForDriver { get; set; }
    }
}
