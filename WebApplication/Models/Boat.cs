using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Boat
    {
        [Key]
        public int BoatId { get; set; }

        public string Name { get; set; }

        public int RegistrationNo { get; set; }

        public double Width { get; set; }

        public double Length { get; set; }

        public double Depth { get; set; }

        public string Type { get; set; }

        [ForeignKey("BoatOwner")]
        public int BoatOwnerId { get; set; }

        public BoatOwner BoatOwner { get; set; }
    }
}