using System.ComponentModel.DataAnnotations;

namespace TruckLink.API.DTOs
{
    public class JobDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Load item is required.")]
        public string LoadItem { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Contact info is required.")]
        public string ContactInfo { get; set; } = null!;

        [Required(ErrorMessage = "Start location is required.")]
        public string StartLocation { get; set; } = null!;

        [Required(ErrorMessage = "Destination is required.")]
        public string Destination { get; set; } = null!;

        [Required(ErrorMessage = "Earnings is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Earnings must be greater than 0.")]
        public decimal Earnings { get; set; }

        [Required(ErrorMessage = "Distance is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Distance must be greater than 0.")]
        public double DistanceKm { get; set; }

        [Required(ErrorMessage = "Map URL is required.")]
        [Url(ErrorMessage = "Map URL must be a valid URL.")]
        public string MapUrl { get; set; } = null!;

        public bool IsCompleted { get; set; } // optional to require this — up to your use case
    }
}
