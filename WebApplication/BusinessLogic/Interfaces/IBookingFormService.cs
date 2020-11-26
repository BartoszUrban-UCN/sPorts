using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingFormService
    {
        Task<Dictionary<int, int>> GetAllAvailableSpotsCount(IList<int> marinaIds, string boatName, DateTime startDate, DateTime endDate);
        Task<IList<Spot>> GetAvailableSpots(int marinaId, string boatName, DateTime startDate, DateTime endDate);

        Task<Booking> CreateBooking();
        Task<BookingLine> GetBookingLine(Booking booking);
        BookingLine UpdateBookingLine(BookingLine bookingLine);
    }
}
