using System.Collections.Generic;

namespace WebApplication.Models
{
    public class MarinaOwner : Role
    {
        public List<Marina> Marinas { get; set; }
    }
}
