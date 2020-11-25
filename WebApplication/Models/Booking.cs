using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int BookingReferenceNo { get; set; }

        [Range(1, 100)]
        [Display(Name = "Total Price")]
        [Required]
        public double TotalPrice { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime CreationDate { get; set; }

        public List<BookingLine> BookingLines { get; set; } = new List<BookingLine>();

        public int BoatId { get; set; }

        [Required]
        public Boat Boat { get; set; }
    }
}
