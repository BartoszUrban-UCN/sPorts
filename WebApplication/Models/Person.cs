using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        public Address Address { get; set; }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }
    }
}
