using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingFormService
    {
        Dictionary<int, int> GetAllAvailableSpotsCount(IList<int> marinaIds, int boatId, DateTime startDate, DateTime endDate);
        IList<Spot> GetAvailableSpots(int marinaId, int boatId, DateTime startDate, DateTime endDate);

        Task<Booking> CreateBooking();
        Task<BookingLine> GetBookingLine(Booking booking);
        BookingLine UpdateBookingLine(BookingLine bookingLine);
    }
}
