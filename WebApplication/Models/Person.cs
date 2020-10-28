using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public Address Address { get; set; }

        public List<Role> Roles { get; set; }

        public void AddRole(Role _role)
        {
            if (Roles == null)
            {
                Roles = new List<Role>();
            }
            Roles.Add(_role);
        }

        public void RemoveRole(Role _role)
        {
            Roles.Remove(_role);
        }
    }
}