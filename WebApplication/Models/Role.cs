using Microsoft.AspNetCore.Identity;

namespace WebApplication.Models
{
    public class Role : IdentityRole<int>
    {
        // I do not yet know how to use roles properly but we need this class
        // for extensibility and scaffolding

        //Calls the base role constructor
        public Role(string roleName) : base(roleName)
        {
        }

        public Role() : base()
        {

        }
    }
}
