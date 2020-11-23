using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic.Shared;

namespace WebApplication.BusinessLogic
{
    public class LoginService : ServiceBase, ILoginService
    {
        public LoginService(SportsContext context) : base(context)
        { }

        public async Task<int> Create(Person person)
        {
            person.ThrowIfNull();

            if (_context.Persons.AsQueryable().Any(p => p.Email.Equals(person.Email)))
            {
                throw new BusinessException("Email", "This email is already taken.");
            }

            await _context.AddAsync(person);

            return person.PersonId;
        }

        public async Task Delete(int? id)
        {
            var person = await GetSingle(id);
            _context.Remove(person);
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();
            return await _context.Persons.AnyAsync(l => l.PersonId == id);
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            var sportsContext = _context.Persons;

            return await sportsContext.ToListAsync();
        }

        public async Task<Person> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var spot = await _context.Persons
                 .FirstOrDefaultAsync(s => s.PersonId == id);

            spot.ThrowIfNull();

            return spot;
        }

        public async Task<BoatOwner> MakePersonBoatOwner(Person person)
        {
            person.ThrowIfNull();

            if (_context.BoatOwners.AsQueryable().Any(p => p.PersonId.Equals(person.PersonId)))
            {
                throw new BusinessException("Email", "You are already registered as a boat owner!");
            }

            var boatOwner = new BoatOwner { PersonId = person.PersonId };

            await _context.BoatOwners.AddAsync(boatOwner);

            return boatOwner;
        }

        public async Task<MarinaOwner> MakePersonMarinaOwner(Person person)
        {
            if (_context.MarinaOwners.AsQueryable().Any(p => p.PersonId.Equals(person.PersonId)))
            {
                throw new BusinessException("Email", "You are already registered as a marina owner!");
            }

            var marinaOwner = new MarinaOwner { PersonId = person.PersonId };

            await _context.MarinaOwners.AddAsync(marinaOwner);

            return marinaOwner;
        }

        public Person Update(Person person)
        {
            person.ThrowIfNull();

            _context.Update(person);
            
            return person;
        }
    }
}
