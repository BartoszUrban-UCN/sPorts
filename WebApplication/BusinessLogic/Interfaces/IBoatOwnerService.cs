using WebApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
namespace WebApplication.BusinessLogic
{
    public interface IBoatOwnerService
    {
        IEnumerable<Booking> OngoingBookings(BoatOwner boatOwner);
        Task<IEnumerable<Booking>> OngoingBookings(int boatOwnerId);
        Task<IEnumerable<Booking>> Bookings(int boatOwnerId);
        Task<BoatOwner> FindBoatOwner(int boatOwnerId);
        bool HasOngoing(Booking booking);
        double MoneySpent(BoatOwner boatOwner);
        TimeSpan TotalTime(BoatOwner boatOwner);
        TimeSpan TotalTime(Booking booking);
        TimeSpan TotalTime(BookingLine bookingLine);
    }
}