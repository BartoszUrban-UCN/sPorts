using Newtonsoft.Json;
using System;
using WebApplication.Models;

namespace WebApplication.BusinessLogic.Shared
{
    public class HelperMethods
    {
        public static string Serialize(object @object)
        {
            return JsonConvert.SerializeObject(@object, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static bool DoesSpotFitBoat(Boat boat, Spot spot)
        {
            var doesSpotFit = true;

            if (spot.MaxDepth < boat.Depth || spot.MaxLength < boat.Length || spot.MaxWidth < boat.Width)
                doesSpotFit = false;

            return doesSpotFit;
        }

        public static bool AreDatesIntersecting(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            var doesDateRangeIntersect = false;

            if (aStart <= bEnd && bStart <= aEnd)
                doesDateRangeIntersect = true;

            return doesDateRangeIntersect;
        }

        public static bool AreDatesNotIntersecting(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            return !AreDatesIntersecting(aStart, aEnd, bStart, bEnd);
        }

        public static bool AreDatesValid(DateTime startDate, DateTime endDate)
        {
            var areDatesValid = false;

            if (endDate >= startDate)
                if (startDate >= DateTime.Today && endDate >= DateTime.Today)
                    areDatesValid = true;

            return areDatesValid;
        }
    }
}
