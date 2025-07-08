namespace TruckLink.API.DTOs
{
    public class JobFilterDto
    {
        public string? Search { get; set; }
        public string? Distance { get; set; } // Assuming in KM or similar
        public string? StartPlace { get; set; }
        public string? EndPlace { get; set; }
    }
}
