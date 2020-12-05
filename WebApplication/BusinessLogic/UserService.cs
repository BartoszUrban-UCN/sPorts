using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class UserService : UserManager<Person>
    {
        private readonly SportsContext _context;

        // Base constructor of UserManager + Context
        public UserService(SportsContext context, IUserStore<Person> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<Person> passwordHasher, IEnumerable<IUserValidator<Person>> userValidators, IEnumerable<IPasswordValidator<Person>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<Person>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _context = context;
        }

        //public override Task<IdentityResult> AddToRoleAsync(Person user, string role)
        //{
        //    if (role == "BoatOwner")
        //    {
        //    }

        //    return base.AddToRoleAsync(user, role);
        //}

        public async Task<BoatOwner> MakePersonBoatOwner(Person person)
        {
            person.ThrowIfNull();

            if (_context.BoatOwners.AsQueryable().Any(p => p.PersonId.Equals(person.Id)))
            {
                throw new BusinessException("Email", "You are already registered as a boat owner!");
            }

            var boatOwner = new BoatOwner { PersonId = person.Id };

            await _context.BoatOwners.AddAsync(boatOwner);

            await AddToRoleAsync(person, "BoatOwner");

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

            await AddToRoleAsync(person, "MarinaOwner");

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

            await RemoveFromRoleAsync(person, "BoatOwner");

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

            await RemoveFromRoleAsync(person, "MarinaOwner");

            return person;
        }

        // Returns the person's associated marina owner, or null if there is none associated
        public MarinaOwner GetMarinaOwnerFromPerson(Person person)
        {
            person.ThrowIfNull();

            var foundMarinaOwner = _context.MarinaOwners.FirstOrDefault(marinaOwner => marinaOwner.PersonId == person.Id);

            return foundMarinaOwner;
        }

        // Returns the person's associated boat owner, or null if there is none associated
        public BoatOwner GetBoatOwnerFromPerson(Person person)
        {
            person.ThrowIfNull();

            var foundBoatOwner = _context.BoatOwners.FirstOrDefault(boatOwner => boatOwner.PersonId == person.Id);

            return foundBoatOwner;
        }
    }
}
