using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface ILoginService : ICRUD<Person>
    {
        Task<BoatOwner> MakePersonBoatOwner(Person person);

        Task<MarinaOwner> MakePersonMarinaOwner(Person person);
    }
}
