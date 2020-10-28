using System;

namespace WebApplication.Models
{
    public class BookingLine
    {
        public int Id { get; set; }

        public double OriginalTotalPrice { get; set; }

        public double AppliedDiscounts { get; set; }

        public double DiscountedTotalPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Confirmed { get; set; }

        public Spot Spot { get; set; }

        public Boat Boat { get; set; }
    }
}
