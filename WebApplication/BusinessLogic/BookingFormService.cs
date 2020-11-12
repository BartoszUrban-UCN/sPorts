using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingFormService : IBookingFormService
    {
        public IList<Spot> GetAvailableSpots(Marina marina, Boat boat, DateTime startDate, DateTime endDate)
        {
            IList<Spot> availableSpots = new List<Spot>();

            // Dates are valid if endDate is later, or on the same day, as startDate and if they are
            // today or later
            if (AreDatesValid(startDate, endDate))
            {
                foreach (Spot spot in marina.Spots)
                {
                    if (DoesSpotFitBoat(boat, spot))
                    {
                        // Only go through Booking Lines that end later than "Now" - does not go
                        // through past bookings
                        foreach (BookingLine bookingLine in spot.BookingLines.Where<BookingLine>(bL => bL.EndDate > DateTime.Now))
                        {
                            if (!DoesDateRangeInsersect(bookingLine.StartDate, bookingLine.EndDate, startDate, endDate))
                            {
                                // Basically returns all spots that
                                // 1. Fit the boat
                                // 2. Have NO date intersects with any existing bookings with no
                                // optimizations in mind whatsoever 🙂
                                availableSpots.Add(spot);
                            }
                        }
                    }
                }
            }

            return availableSpots;
        }

        public bool DoesSpotFitBoat(Boat boat, Spot spot)
        {
            var doesSpotFit = true;
            if (spot.MaxDepth < boat.Depth || spot.MaxLength < boat.Length || spot.MaxWidth < boat.Width)
            {
                doesSpotFit = false;
            }
            return doesSpotFit;
        }

        public bool DoesDateRangeInsersect(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            var doesDateRangeIntersect = false;

            if (aStart < bEnd && bStart < aEnd)
            {
                doesDateRangeIntersect = true;
            }

            return doesDateRangeIntersect;
        }

        public bool AreDatesValid(DateTime startDate, DateTime endDate)
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
    }
}
