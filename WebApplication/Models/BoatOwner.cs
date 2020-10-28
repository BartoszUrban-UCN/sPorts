using System.Collections.Generic;

namespace WebApplication.Models
{
    public class BoatOwner : Person
    {
        public List<Boat> Boats { get; set; }
    }
}