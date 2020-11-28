using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
        public string IncomingPaymentReference { get; set; }
        public string IncomingPaymentStatus { get; set; }
        public string InvoiceStatus { get; set; }

    }
}
