using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic.Interfaces
{
    public interface IPaymentService: ICRUD<Payment>
    {
        Task<Payment> CreateFromBooking(Booking booking);
        Task<Payment> StartPayment(Payment payment);
    }
}
