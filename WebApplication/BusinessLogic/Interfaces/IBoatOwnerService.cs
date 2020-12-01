using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;
namespace WebApplication.BusinessLogic
{
    public interface IBoatOwnerService : ICRUD<BoatOwner>
    {
        Task<IEnumerable<Booking>> GetOngoingBookings(int? id);
        Task<IEnumerable<Booking>> GetBookings(int? id);
        Task<IEnumerable<Boat>> GetBoats(int? id);
        Task<IEnumerable<BookingLine>> GetBookingLines(int? bookingId);
        Task<IEnumerable<BookingLine>> GetOngoingBookingLines(int? bookingId);
        bool HasOngoing(Booking booking);
        double MoneySpent(BoatOwner boatOwner);
        TimeSpan TotalTime(BoatOwner boatOwner);
        TimeSpan TotalTime(Booking booking);
        TimeSpan TotalTime(BookingLine bookingLine);
    }
}