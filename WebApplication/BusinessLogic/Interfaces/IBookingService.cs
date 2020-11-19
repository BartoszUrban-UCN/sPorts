using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingService : ICRUD<Booking>, IBookingConfirmationService, IBookingLineService
    {
        List<BookingLine> CreateBookingLines(Dictionary<DateTime[], Spot> marinaSpotStayDates);

        Booking ExplicitLoad(Booking booking);

        Task<BookingLine> GetBookingLine(int id);

        Task<bool> CancelBooking(int id);

        Task<List<BookingLine>> GetUnconfirmedBookingLines(int marinaOwnerId);
    }
}
