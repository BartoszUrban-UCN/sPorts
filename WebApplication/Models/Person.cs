using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        [ForeignKey("Address")]
        public int AddressId { get; set; }

        public Address Address { get; set; }

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