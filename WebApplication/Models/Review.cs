using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public byte Starts { get; set; }

        public string Comment { get; set; }

        [ForeignKey("Marina")]
        public int MarinaId { get; set; }

        public Marina Marina { get; set; }
    }
}