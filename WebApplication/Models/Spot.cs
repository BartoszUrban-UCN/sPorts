using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class Spot
    {
        public int SpotId { get; set; }

        [Required]
        public int SpotNumber { get; set; }

        [Required]
        public bool Available { get; set; }

        [Required]
        [Range(1, 40, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double MaxWidth { get; set; }

        [Required]
        [Range(1, 40, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double MaxLength { get; set; }

        [Required]
        [Range(1, 40, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double MaxDepth { get; set; }

        [Required]
        public double Price { get; set; }

        public int? LocationId { get; set; }

        public Location Location { get; set; }

        public int? MarinaId { get; set; }
        public Marina Marina { get; set; }

        public List<BookingLine> BookingLines { get; set; } = new List<BookingLine>();
    }
}
