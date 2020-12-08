using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;
using WebApplication.Authorization;

namespace WebApplication.Data
{
    public static class RoleInitializer
    {
        // These are the role names we want to always have in the database at the start
        private static readonly string[] RoleNames = {
            RoleName.BoatOwner,
            RoleName.MarinaOwner,
            RoleName.Manager,
            RoleName.Administrator
        };

        // Seed roles in the db if they don't exist
        public static async Task SeedRoles(IServiceProvider serviceProvider, SportsContext context)
        {
            if (!context.UserRoles.Any())
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

                foreach (var role in RoleNames)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new Role(role));
                    }
                }
            }
        }
    }
}
