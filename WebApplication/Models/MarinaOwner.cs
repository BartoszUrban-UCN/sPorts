using System.Collections.Generic;

namespace WebApplication.Models
{
    public class MarinaOwner
    {
        public int MarinaOwnerId { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }

        public List<Marina> Marina { get; set; } = new List<Marina>();
    }
}
