using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class BookingLine
    {
        [Key]
        public int Id { get; set; }

        public double OriginalTotalPrice { get; set; }

        public double AppliedDiscounts { get; set; }

        public double DiscountedTotalPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Confirmed { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        [ForeignKey("Spot")]
        public int SpotId { get; set; }

        [ForeignKey("Boat")]
        public int BoatId { get; set; }

        public Booking Booking { get; set; }

        public Spot Spot { get; set; }

        public Boat Boat { get; set; }
    }
}
