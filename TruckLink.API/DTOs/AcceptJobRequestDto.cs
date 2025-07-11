using System.ComponentModel.DataAnnotations;

namespace TruckLink.API.DTOs
{
    public class AcceptJobRequestDto
    {
        [Required]
        public Guid JobId { get; set; }

        [Required]
        public Guid DriverId { get; set; }
    }
}
