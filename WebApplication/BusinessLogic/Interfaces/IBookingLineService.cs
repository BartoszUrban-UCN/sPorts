using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingLineService : ICRUD<BookingLine>
    {
        Task CancelBookingLine(int? id);

        Task AddTime(int? id, int amount);
    }
}
