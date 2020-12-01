using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Interfaces;
using WebApplication.Models;
using WebApplication.Data;
using WebApplication.BusinessLogic.Shared;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.BusinessLogic
{
    public class PaymentService : ServiceBase, IPaymentService
    {
        public PaymentService(SportsContext context) : base(context)
        { }
        public async Task<Payment> CreateFromBooking(Booking booking)
        {
            booking.ThrowIfNull();

            Payment payment = new Payment();


            payment.BookingId = booking.BookingId;
            payment.Amount = booking.TotalPrice;


            return payment;
            
        }

        public async Task<int> Create(Payment payment)
        {
            payment.ThrowIfNull();

            await _context.AddAsync(payment);

            return payment.PaymentId;
        }

        public async Task Delete(int? id)
        {
            var payment = await GetSingle(id);
            _context.Payments.Remove(payment);
        }

        public Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();
            return _context.Payments.AnyAsync(p => p.PaymentId == id);
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            var payments = await _context.Payments
                                         .Include(p => p.Booking)
                                         .ToListAsync();
            return payments;
        }

        public async Task<Payment> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var payment = await _context.Payments
                .Include(p => p.BookingId)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            payment.ThrowIfNull();

            return payment;
        }

        public Payment Update(Payment payment)
        {
            _context.Update(payment);

            return payment;
        }

        public async Task<Payment> StartPayment(Payment payment)
        {
            
            payment.ThrowIfNull();

            var paymentProvider = new MockPayProvider();
           
            string result = paymentProvider.ProcessPayment();

            payment.IncomingPaymentStatus = result;
            
            return payment;
        }

        
    }
}
