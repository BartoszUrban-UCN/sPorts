using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Spot
    {
        [Key]
        public int SpotId { get; set; }

        public int SpotNumber { get; set; }

        public bool Available { get; set; }

        public double MaxWidth { get; set; }

        public double MaxLength { get; set; }

        public double MaxDepth { get; set; }

        public double Price { get; set; }

        [ForeignKey("Marina")]
        public int MarinaId { get; set; }

        public Marina Marina { get; set; }
    }
}