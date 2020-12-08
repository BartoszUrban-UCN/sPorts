using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingFormService
    {
        Task<IEnumerable<KeyValuePair<Marina, int>>> GetAllAvailableSpotsCount(IList<int> marinaIds, int boatId, DateTime startDate, DateTime endDate);

        Task<IList<Spot>> GetAvailableSpots(int marinaId, int boatId, DateTime startDate, DateTime endDate);
    }
}
