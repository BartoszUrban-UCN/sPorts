using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingFormService
    {
        public Dictionary<int, int> GetAllAvailableSpotsCount(IList<int> marinaIds, int boatId, DateTime startDate, DateTime endDate);
        public IList<Spot> GetAvailableSpots(int marinaId, int boatId, DateTime startDate, DateTime endDate);
    }
}
