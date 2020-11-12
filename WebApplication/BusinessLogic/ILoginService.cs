using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface ILoginService
    {
        Task<bool> CreatePerson(Person person);

        Task<bool> MakePersonBoatOwner(Person person);

        Task<bool> MakePersonMarinaOwner(Person person);
    }
}
