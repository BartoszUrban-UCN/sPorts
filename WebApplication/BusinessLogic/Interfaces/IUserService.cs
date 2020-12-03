using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IUserService
    {
        Task<BoatOwner> MakePersonBoatOwner(Person person);

        Task<MarinaOwner> MakePersonMarinaOwner(Person person);

        Task<Person> RevokeBoatOwnerRights(Person person);

        Task<Person> RevokeMarinaOwnerRights(Person person);
    }
}
