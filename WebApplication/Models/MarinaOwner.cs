using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class MarinaOwner
    {
        public int MarinaOwnerId { get; set; }

        public int PersonId { get; set; }

        public Person Person { get; set; }

        public List<Spot> Spots { get; set; }
    }
}
