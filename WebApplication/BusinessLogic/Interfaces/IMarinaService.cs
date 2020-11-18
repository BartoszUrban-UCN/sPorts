using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IMarinaService : ICRUD<Marina>
    {
        Task<int> CreateWithLocation(Marina marina, Location location);
        Task<Spot> UpdateMarinaLocation(Marina marina, Location location);
        Task DeleteMarinaLocation(Marina marina);
    }
}
