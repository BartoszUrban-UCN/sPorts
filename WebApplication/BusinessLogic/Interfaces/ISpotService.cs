using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface ISpotService : ICRUD<Spot>
    {
        Task<int> CreateWithLocation(Spot spot, Location location);
        Task<Spot> UpdateSpotLocation(Spot spot, Location location);
        Task DeleteSpotLocation(Spot spot);
    }
}
