using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class Marina
    {
        public int MarinaId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Facilities { get; set; }

        public int MarinaOwnerId { get; set; }
        public MarinaOwner MarinaOwner { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public int? LocationId { get; set; }
        public Location Location { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();

        public List<Spot> Spots { get; set; } = new List<Spot>();
    }
}
