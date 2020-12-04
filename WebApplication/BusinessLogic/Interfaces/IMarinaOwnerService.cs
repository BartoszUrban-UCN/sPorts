using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;
namespace WebApplication.BusinessLogic
{
    public interface IMarinaOwnerService : ICRUD<MarinaOwner>
    {
        Task<IEnumerable<BookingLine>> GetConfirmedBookingLines(int marinaOwnerId);

        Task<IEnumerable<BookingLine>> GetUnconfirmedBookingLines(int marinaOwnerId);
    }
}