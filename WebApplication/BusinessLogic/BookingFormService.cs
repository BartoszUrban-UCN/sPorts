using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingFormService : ServiceBase, IBookingFormService
    {
        public BookingFormService(SportsContext context) : base(context)
        {
        }

        public async Task<Dictionary<int, int>> GetAllAvailableSpotsCount(IList<int> marinaIds, int boatId, DateTime startDate, DateTime endDate)
        {
            var availableSpotsPerMarinaId = new Dictionary<int, int>();

            foreach (var marinaId in marinaIds)
            {
                var availableSpotsInMarina = await GetAvailableSpots(marinaId, boatId, startDate, endDate);

                if (availableSpotsInMarina.Any())
                {
                    availableSpotsPerMarinaId.Add(marinaId, availableSpotsInMarina.Count);
                }
            }

            return availableSpotsPerMarinaId;
        }

        public async Task<IList<Spot>> GetAvailableSpots(int marinaId, int boatId, DateTime startDate, DateTime endDate)
        {
            IList<Spot> availableSpots = new List<Spot>();

            var marina = _context.Marinas
                .Include(marina => marina.Location)
                .Include(marina => marina.Spots)
                .ThenInclude(spot => spot.Location)
                .FirstOrDefault(marina => marina.MarinaId == marinaId);

            var boat = _context.Boats.FindAsync(boatId);

            // Dates are valid if endDate is later, or on the same day, as startDate and if they are
            // today or later
            if (AreDatesValid(startDate, endDate))
            {
                foreach (Spot spot in marina.Spots)
                {
                    if (DoesSpotFitBoat(await boat, spot))
                    {
                        // Only go through Booking Lines that end later than "Now" - does not go
                        // through past bookings

                        var booked = false;
                        foreach (BookingLine bookingLine in spot.BookingLines.Where<BookingLine>(bL => bL.EndDate > DateTime.Now))
                        {
                            if (DoesDateRangeInsersect(bookingLine.StartDate, bookingLine.EndDate, startDate, endDate))
                            {
                                // Basically returns all spots that
                                // 1. Fit the boat
                                // 2. Have NO date intersects with any existing bookings with no
                                // optimizations in mind whatsoever 🙂
                                booked = true;
                                break;
                            }
                        }

                        if (!booked)
                        {
                            availableSpots.Add(spot);
                        }
                    }
                }
            }

            return availableSpots;
        }

        public static bool DoesSpotFitBoat(Boat boat, Spot spot)
        {
            var doesSpotFit = true;
            if (spot.MaxDepth < boat.Depth || spot.MaxLength < boat.Length || spot.MaxWidth < boat.Width)
            {
                doesSpotFit = false;
            }
            return doesSpotFit;
        }

        public static bool DoesDateRangeInsersect(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            var doesDateRangeIntersect = false;

            if (aStart < bEnd && bStart < aEnd)
            {
                doesDateRangeIntersect = true;
            }

            return doesDateRangeIntersect;
        }

        public static bool AreDatesValid(DateTime startDate, DateTime endDate)
        {
            var areDatesValid = false;

            if (endDate >= startDate)
            {
                if (startDate >= DateTime.Today && endDate >= DateTime.Today)
                {
                    areDatesValid = true;
                }
            }

            return areDatesValid;
        }

        // ??
        public async Task<Booking> CreateBooking()
        {
            var booking = new Booking { CreationDate = DateTime.Now, BookingReferenceNo = 123 };
            var bookingLine = new BookingLine { Booking = booking };
            booking.BookingLines.Add(bookingLine);

            _context.Add(booking);
            return booking;
        }

        public async Task<BookingLine> GetBookingLine(Booking booking)
        {
            return (await _context.Bookings.FindAsync(booking.BookingId)).BookingLines.Last();
        }

        public BookingLine UpdateBookingLine(BookingLine bookingLine)
        {
            _context.BookingLines.Update(bookingLine);
            return bookingLine;
        }
    }
}
