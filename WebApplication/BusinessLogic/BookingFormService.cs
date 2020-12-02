using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingFormService : ServiceBase, IBookingFormService
    {
        private readonly IMarinaService _marinaService;

        public BookingFormService(SportsContext context, IMarinaService marinaService) : base(context)
        {
            _marinaService = marinaService;
        }

        public async Task<IEnumerable<KeyValuePair<Marina, int>>> GetAllAvailableSpotsCount(IList<int> marinaIds, int boatId, DateTime startDate, DateTime endDate)
        {
            var availableSpotsPerMarinaId = new List<KeyValuePair<Marina, int>>();

            foreach (var marinaId in marinaIds)
            {
                var availableSpotsInMarina = await GetAvailableSpots(marinaId, boatId, startDate, endDate);

                if (availableSpotsInMarina.Any())
                {
                    availableSpotsPerMarinaId.Add(new KeyValuePair<Marina, int>(availableSpotsInMarina.First().Marina, availableSpotsInMarina.Count));
                }
            }

            return availableSpotsPerMarinaId;
        }

        public async Task<IList<Spot>> GetAvailableSpots(int marinaId, int boatId, DateTime startDate, DateTime endDate)
        {
            IList<Spot> availableSpots = new List<Spot>();

            var marina = await _marinaService.GetSingle(marinaId);

            var boat = await _context.Boats.FindAsync(boatId);

            // Dates are valid if endDate is later, or on the same day, as startDate and if they are
            // today or later
            if (HelperMethods.AreDatesValid(startDate, endDate))
            {
                foreach (Spot spot in marina.Spots)
                {
                    if (spot.Available)
                    {
                        if (HelperMethods.DoesSpotFitBoat(boat, spot))
                        {
                            // Only go through Booking Lines that end later than "Now" - does not go
                            // through past bookings

                            var booked = false;
                            foreach (BookingLine bookingLine in spot.BookingLines.Where<BookingLine>(bL => bL.EndDate > DateTime.Now))
                                if (HelperMethods.AreDatesIntersecting(bookingLine.StartDate, bookingLine.EndDate, startDate, endDate))
                                {
                                    // Basically returns all spots that
                                    // 1. Fit the boat
                                    // 2. Have NO date intersects with any existing bookings with no
                                    // optimizations in mind whatsoever 🙂
                                    booked = true;
                                    break;
                                }

                            if (!booked)
                            {
                                availableSpots.Add(spot);
                            }
                        }
                    }
                }
            }

            return availableSpots;
        }
    }
}
