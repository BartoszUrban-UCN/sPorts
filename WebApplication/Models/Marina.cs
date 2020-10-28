using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Marina
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Facilities { get; set; }

        public MarinaOwner MarinaOwner { get; set; }

        public Address Address { get; set; }

        public List<Review> Reviews { get; set; }

        public List<Spot> Spots { get; set; }
    }
}