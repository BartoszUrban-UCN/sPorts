using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingConfirmationService
    {
        Task<bool> ConfirmSpotBooked(int bookingLineId);
        Task<List<BookingLine>> GetBookingsByMarinaOwner(MarinaOwner marinaOwner);
        Task<List<BookingLine>> GetUnconfirmedBookingLines(MarinaOwner marinaOwner);
        void SendConfirmationMail();
    }
}