using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Business_Logic
{
    public class LoginService
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
                throw new ArgumentException("A person with this email already exists");
            }

            _context.Persons.Add(person);
            success = await _context.SaveChangesAsync() > 0;

            return success;
        }
    }
}
