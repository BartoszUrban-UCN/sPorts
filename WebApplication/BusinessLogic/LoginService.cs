using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class LoginService : ILoginService
    {
        private readonly SportsContext _context;

        public LoginService(SportsContext context)
        {
            _context = context;
        }

        public async Task<bool> CreatePerson(Person person)
        {
            var success = false;
            if (_context.Persons.AsQueryable().Any(p => p.Email.Equals(person.Email)))
            {
                throw new BusinessException("Email", "This email is already taken.");
            }

            _context.Persons.Add(person);

            success = await _context.SaveChangesAsync() > 0;

            return success;
        }

        public async Task<bool> MakePersonBoatOwner(Person person)
        {
            var success = false;

            if (_context.BoatOwners.AsQueryable().Any(p => p.PersonId.Equals(person.PersonId)))
            {
                throw new BusinessException("Email", "You are already registered as a boat owner!");
            }

            var boatOwner = new BoatOwner { PersonId = person.PersonId };

            _context.BoatOwners.Add(boatOwner);
            success = await _context.SaveChangesAsync() > 0;

            return success;
        }

        public async Task<bool> MakePersonMarinaOwner(Person person)
        {
            var success = false;

            if (_context.MarinaOwners.AsQueryable().Any(p => p.PersonId.Equals(person.PersonId)))
            {
                throw new BusinessException("Email", "You are already registered as a marina owner!");
            }

            var marinaOwner = new MarinaOwner { PersonId = person.PersonId };

            _context.MarinaOwners.Add(marinaOwner);
            success = await _context.SaveChangesAsync() > 0;

            return success;
        }
    }
}
