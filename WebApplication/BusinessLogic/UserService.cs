using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public static class UserService : ServiceBase, IUserService
    {
        private readonly UserManager<Person> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserService(SportsContext context, UserManager<Person> userManager, RoleManager<Role> roleManager) : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<int> Create(Person person)
        {
            person.ThrowIfNull();

            if (_context.Persons.AsQueryable().Any(p => p.Email.Equals(person.Email)))
            {
                throw new BusinessException("Email", "This email is already taken.");
            }

            await _context.AddAsync(person);

            return person.Id;
        }

        public async Task Delete(int? id)
        {
            var person = await GetSingle(id);
            _context.Remove(person);
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();
            return await _context.Persons.AnyAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            var sportsContext = _context.Persons;

            return await sportsContext.ToListAsync();
        }

        public async Task<Person> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var person = await _context.Persons
                 .FirstOrDefaultAsync(s => s.Id == id);

            person.ThrowIfNull();

            return person;
        }

        public async Task<BoatOwner> MakePersonBoatOwner(Person person)
        {
            person.ThrowIfNull();

            if (_context.BoatOwners.AsQueryable().Any(p => p.PersonId.Equals(person.Id)))
            {
                throw new BusinessException("Email", "You are already registered as a boat owner!");
            }

            var boatOwner = new BoatOwner { PersonId = person.Id };

            await _context.BoatOwners.AddAsync(boatOwner);

            await _userManager.AddToRoleAsync(person, "BoatOwner");

            return boatOwner;
        }

        public async Task<MarinaOwner> MakePersonMarinaOwner(Person person)
        {
            person.ThrowIfNull();

            if (_context.MarinaOwners.AsQueryable().Any(p => p.PersonId.Equals(person.Id)))
            {
                throw new BusinessException("Email", "You are already registered as a marina owner!");
            }

            var marinaOwner = new MarinaOwner { PersonId = person.Id };

            await _context.MarinaOwners.AddAsync(marinaOwner);

            await _userManager.AddToRoleAsync(person, "MarinaOwner");

            return marinaOwner;
        }

        public async Task<Person> RevokeBoatOwnerRights(Person person)
        {
            person.ThrowIfNull();

            var boatOwner = await _context.BoatOwners.FirstOrDefaultAsync(foundBoatOwner => foundBoatOwner.PersonId.Equals(person.Id));

            if (boatOwner is not null)
            {
                person.BoatOwner = null;
                _context.BoatOwners.Remove(boatOwner);
            }

            await _userManager.RemoveFromRoleAsync(person, "BoatOwner");

            return person;
        }

        public async Task<Person> RevokeMarinaOwnerRights(Person person)
        {
            person.ThrowIfNull();

            var marinaOwner = await _context.MarinaOwners.FirstOrDefaultAsync(foundMarinaOwner => foundMarinaOwner.PersonId.Equals(person.Id));

            if (marinaOwner is not null)
            {
                person.MarinaOwner = null;
                _context.MarinaOwners.Remove(marinaOwner);
            }

            await _userManager.RemoveFromRoleAsync(person, "MarinaOwner");

            return person;
        }

        public Person Update(Person person)
        {
            person.ThrowIfNull();

            _context.Update(person);

            return person;
        }
    }
}
