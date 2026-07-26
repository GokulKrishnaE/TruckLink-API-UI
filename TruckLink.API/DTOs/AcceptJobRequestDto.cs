using System.ComponentModel.DataAnnotations;

namespace TruckLink.API.DTOs
{
    public class AcceptJobRequestDto
    {
        [Required]
        public int JobId { get; set; }

        [Required]
        public int DriverId { get; set; }
    }
}
