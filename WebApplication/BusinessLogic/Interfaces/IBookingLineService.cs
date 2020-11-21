using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingLineService : ICRUD<BookingLine>
    {
        Task<bool> CancelBookingLine(int? id);
        Task<bool> AddTime(int? id, int amount);
    }
}
