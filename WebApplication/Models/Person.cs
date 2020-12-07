using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class Person : IdentityUser<int>
    {
        [Required]
        [PersonalData]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        public string LastName { get; set; }

        public int? AddressId { get; set; }

        public Address Address { get; set; }
        public BoatOwner BoatOwner { get; set; }
        public MarinaOwner MarinaOwner { get; set; }
    }
}
