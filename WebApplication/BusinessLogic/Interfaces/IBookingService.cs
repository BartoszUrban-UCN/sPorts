using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingService
    {
        Task<bool> CreateBooking(Booking booking);

        List<BookingLine> CreateBookingLines(Dictionary<DateTime[], Spot> marinaSpotStayDates);

        Task<IEnumerable<BookingLine>> GetBookingLines(int bookingId);

        Task<bool> CancelBooking(int id);
    }
}
