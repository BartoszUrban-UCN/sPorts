using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingFormService
    {
        public IList<Spot> GetAvailableSpots(Marina marina, Boat boat, DateTime startDate, DateTime endDate);

        public bool DoesSpotFitBoat(Boat boat, Spot spot);
    }
}
