using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class BoatOwner
    {
        public int BoatOwnerId { get; set; }

        public int PersonId { get; set; }

        public Person Person { get; set; }

        public List<Boat> Boats { get; set; }
    }
}
