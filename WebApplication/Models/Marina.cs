using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Marina
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Facilities { get; set; }

        [ForeignKey("MarinaOwner")]
        public int MarinaOwnerId { get; set; }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }

        public MarinaOwner MarinaOwner { get; set; }

        public Address Address { get; set; }

        public List<Review> Reviews { get; set; }

        public List<Spot> Spots { get; set; }
    }
}
