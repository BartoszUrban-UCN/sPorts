using System.Collections.Generic;

namespace WebApplication.Models
{
    public class BoatOwner : Role
    {
        public List<Boat> Boats { get; set; }
    }
}