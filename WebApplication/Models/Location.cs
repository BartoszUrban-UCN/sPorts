namespace WebApplication.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }

        public Spot Spot { get; set; }
        public Marina Marina { get; set; }
    }
}
