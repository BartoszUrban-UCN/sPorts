﻿using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int BookingReferenceNo { get; set; }

        public double TotalPrice { get; set; }

        public string PaymentStatus { get; set; }

        public List<BookingLine> BookingLines { get; set; }
    }
}
