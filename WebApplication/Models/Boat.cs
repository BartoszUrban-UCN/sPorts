using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Boat
    {
        public int BoatId { get; set; }
        public string Name { get; set; }
        public int RegistrationNo { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Depth { get; set; }
        public string Type { get; set; }
        public int BoatOwnerId { get; set; }
        public BoatOwner BoatOwner { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
