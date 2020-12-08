using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface ISpotService : ICRUD<Spot>
    {
        Task<IEnumerable<Spot>> GetAll(int marinaOwnerId);
        Task<int> CreateWithLocation(Spot spot, Location location);
        Task<Spot> UpdateSpotLocation(Spot spot, Location location);
        Task<Spot> DeleteSpotLocation(Spot spot);
    }
}
