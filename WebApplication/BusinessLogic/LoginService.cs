using System;
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

        public bool CreatePerson(Person person)
        {
            var success = false;

            if (_context.Persons.Find(person.PersonId) != null)
            {
                throw new ArgumentException("Do not specify idenity");
            }

            return success;
        }
    }
}
