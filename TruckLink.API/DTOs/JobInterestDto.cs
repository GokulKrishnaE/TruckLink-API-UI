using System.ComponentModel.DataAnnotations;

namespace TruckLink.API.DTOs
{
    public class JobInterestDto
    {
        [Required(ErrorMessage = "Driver ID is required.")]
        public Guid DriverId { get; set; }

        [Required(ErrorMessage = "Driver name is required.")]
        [StringLength(100, ErrorMessage = "Driver name cannot exceed 100 characters.")]
        public string DriverName { get; set; } = null!;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must be a valid 10-digit Indian number.")]
        public string MobileNumber { get; set; } = null!;

        [Required(ErrorMessage = "Requested time is required.")]
        public DateTime RequestedAt { get; set; }
    }
}
