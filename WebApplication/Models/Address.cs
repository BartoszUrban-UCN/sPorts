using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }
    }
}
