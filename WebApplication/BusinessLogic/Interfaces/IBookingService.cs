using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingService : ICRUD<Booking>
    {
        List<BookingLine> CreateBookingLines(Dictionary<DateTime[], Spot> marinaSpotStayDates);

        Task<BookingLine> GetBookingLine(int? id);

        // Task<List<BookingLine>> GetUnconfirmedBookingLines(int? marinaOwnerId);

        Task<IEnumerable<BookingLine>> GetBookingLines(int? id);

        Task<IEnumerable<BookingLine>> GetOngoingBookingLines(int? id);

        Task<bool> ConfirmSpotBooked(int bookingLineId);

        Task<bool> CancelBooking(int? id);
    }
}
