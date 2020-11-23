using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBoatService : ICRUD<Boat>
    {
        Task<Boat> GetSingleByName(string name);
    }
}
