using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingFormService
    {
        public Spot GetFirstAvailableSpot(Marina marina, Boat boat, DateTime startDate, DateTime endDate)
        {
            //var isAnySpotAvailable = false;
            //Spot foundSpot = null;

            foreach (Spot spot in marina.Spots)
            {
                var isSpotGood = true;

                // Check whether the boat "fits"
                isSpotGood = DoesSpotFitBoat(boat, spot);

                if (isSpotGood)
                {
                    foreach (BookingLine bookingLine in spot.BookingLines)
                    {
                        if (!DateRangeIntersects(bookingLine.StartDate, bookingLine.EndDate, startDate, endDate))
                        {
                            return spot;
                        }
                    }
                }
            }

            return null;
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

        public static bool DateRangeIntersects(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            var intersects = false;

            if (aStart < bEnd && bStart < aEnd)
            {
                intersects = true;
            }

            return intersects;
        }
    }
}
