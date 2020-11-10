using System;

namespace WebApplication.Models
{
    public class BookingLine
    {
        public int BookingLineId { get; set; }

        public double OriginalTotalPrice { get; set; }

        public double AppliedDiscounts { get; set; }

        public double DiscountedTotalPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Confirmed { get; set; }

        public int BookingId { get; set; }

        public int SpotId { get; set; }

        public bool Ongoing { get; set; }

        public Booking Booking { get; set; }

        public Spot Spot { get; set; }
    }
}
