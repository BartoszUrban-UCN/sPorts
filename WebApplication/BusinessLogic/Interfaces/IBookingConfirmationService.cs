using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingConfirmationService
    {
        Task<bool> ConfirmSpotBooked(int bookingLineId);
        Task<List<BookingLine>> GetBookingLinesByMarinaOwner(int marinaOwnerId);
        Task<List<BookingLine>> GetUnconfirmedBookingLines(int marinaOwnerId);
        void SendConfirmationMail(int bookingId);
    }
}