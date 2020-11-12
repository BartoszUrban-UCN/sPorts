using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BoatOwnerService
    {
        private readonly SportsContext _context;

        public BoatOwnerService(SportsContext context)
        {
            _context = context;
        }

        public IEnumerable<Booking> OngoingBookings(BoatOwner boatOwner)
        {
            if (boatOwner == null)
            {
                throw new System.ArgumentNullException();
            }

            var boats = boatOwner.Boats;
            var bookingsToReturn = from boat in boats
                                   from booking in boat.Bookings
                                   where HasOngoing(booking)
                                   select booking;


            return bookingsToReturn;
        }

        public bool HasOngoing(Booking booking)
        {
            foreach (var bookingLine in booking.BookingLines)
            {
                if (bookingLine.Ongoing)
                    return true;
            }
            return false;
        }
    }
}
