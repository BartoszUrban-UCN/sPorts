using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IMarinaService : ICRUD<Marina>
    {
        Task<IEnumerable<Marina>> GetAll(int marinaOwnerId);
        Task<int> CreateWithLocation(Marina marina, Location location);
        Task<int> CreateLocationForMarina(Marina marina, Location marinaLocation);
        Marina UpdateMarinaLocation(Marina marina, Location location);
        Task<Marina> DeleteMarinaLocation(Marina marina);
        Task<bool> NotExists(int? id);
    }
}
