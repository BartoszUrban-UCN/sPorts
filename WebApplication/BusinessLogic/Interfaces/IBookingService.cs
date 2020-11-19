using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingService : ICRUD<Booking>
    {
        List<BookingLine> CreateBookingLines(Dictionary<DateTime[], Spot> marinaSpotStayDates);

        Booking ExplicitLoad(Booking booking);

        //Task<IEnumerable<BookingLine>> GetBookingLines(int bookingId);

        //Task<bool> CancelBooking(int id);
    }
}
