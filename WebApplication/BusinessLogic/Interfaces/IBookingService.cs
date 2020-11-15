using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingService
    {
        Task<bool> CreateBooking(BoatOwner boatOwner, Boat boat, Dictionary<DateTime[], Spot> marinaSpotStayDates);

        Task<IList<BookingLine>> GetBookingLines(int bookingId);

        void DeleteBookingFiles(int bookingReferenceNo);

        void AddTimeToBookingLine(BookingLine bookingLine, int seconds);
    }
}
