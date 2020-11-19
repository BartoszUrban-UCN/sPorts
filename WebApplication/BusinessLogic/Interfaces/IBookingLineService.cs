using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingLineService
    {
        Task<List<BookingLine>> GetBookingLinesByMarinaOwner(int marinaOwnerId);

        Task<BookingLine> FindBookingLine(int id);


        Task<bool> CancelBookingLine(int id);

        Task<bool> AddTime(int id, int amount);
    }
}
