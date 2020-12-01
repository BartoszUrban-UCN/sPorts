using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class Person
    {
        public int PersonId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        //[RegularExpression("\\p{L}")]
        public string FirstName { get; set; }

        [Required]
        //[RegularExpression("\\p{L}")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public Address Address { get; set; }

        public int? AddressId { get; set; }
    }
}
