using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Data;
using WebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.BusinessLogic
{
    public class BoatOwnerService
    {
        private readonly SportsContext _context;

        public BoatOwnerService(SportsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> OngoingBookings(int boatOwnerId)
        {
            var boatWithBooking = _context.Boats.Include(b => b.Bookings)
                                                .ThenInclude(b => b.BookingLines);
            var boatList = await boatWithBooking.ToListAsync();
            var boat = boatList.Find(b => b.BoatId == boatOwnerId);

            if (boat != null)
            {
                var bookingsToReturn = new List<Booking>();
                var bookings = boat.Bookings;

                foreach (var booking in bookings)
                {
                    if (HasOngoing(booking))
                    {
                        bookingsToReturn.Add(booking);
                    }
                }

                return bookingsToReturn;
            }
            else
            {
                throw new System.ArgumentNullException();
            }
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
