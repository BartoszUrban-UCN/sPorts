﻿using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Marina
    {
        public int MarinaId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Facilities { get; set; }

        public int MarinaOwnerId { get; set; }
        public MarinaOwner MarinaOwner { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public List<Review> Reviews { get; set; }

        public List<Spot> Spots { get; set; }
    }
}
