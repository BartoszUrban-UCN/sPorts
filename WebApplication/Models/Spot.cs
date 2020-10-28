namespace WebApplication.Models
{
    public class Spot
    {
        public int Id { get; set; }

        public int SpotNumber { get; set; }

        public bool Available { get; set; }

        public double MaxWidth { get; set; }

        public double MaxLength { get; set; }

        public double MaxDepth { get; set; }

        public double Price { get; set; }

        public Marina Marina { get; set; }
    }
}