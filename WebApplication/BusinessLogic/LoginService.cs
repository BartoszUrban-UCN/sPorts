using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class LoginService : ServiceBase, ILoginService
    {
        public LoginService(SportsContext context) : base(context)
        {
        }

        public async Task<int> Create(Person person)
        {
            if (_context.Persons.AsQueryable().Any(p => p.Email.Equals(person.Email)))
            {
                throw new BusinessException("Email", "This email is already taken.");
            }

            _context.Add(person);
            return person.PersonId;
        }

        public async Task Delete(int? id)
        {
            var person = await _context.Persons.FindAsync(id);
            _context.Remove(person);
        }

        public Task<bool> Exists(int? id)
        {
            return _context.Persons.AnyAsync(l => l.PersonId == id);
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            var sportsContext = _context.Persons;

            return await sportsContext.ToListAsync();
        }

        public async Task<Person> GetSingle(int? id)
        {
            var spot = _context.Persons
                 .FirstOrDefaultAsync(s => s.PersonId == id);

            return await spot;
        }

        public async Task<BoatOwner> MakePersonBoatOwner(Person person)
        {
            if (_context.BoatOwners.AsQueryable().Any(p => p.PersonId.Equals(person.PersonId)))
            {
                throw new BusinessException("Email", "You are already registered as a boat owner!");
            }

            var boatOwner = new BoatOwner { PersonId = person.PersonId };

            _context.BoatOwners.Add(boatOwner);
            await Save();

            return boatOwner;
        }

        public async Task<MarinaOwner> MakePersonMarinaOwner(Person person)
        {
            if (_context.MarinaOwners.AsQueryable().Any(p => p.PersonId.Equals(person.PersonId)))
            {
                throw new BusinessException("Email", "You are already registered as a marina owner!");
            }

            var marinaOwner = new MarinaOwner { PersonId = person.PersonId };

            _context.MarinaOwners.Add(marinaOwner);
            await Save();

            return marinaOwner;
        }

        public async Task<Person> Update(Person person)
        {
            _context.Update(person);
            return person;
        }
    }
}
