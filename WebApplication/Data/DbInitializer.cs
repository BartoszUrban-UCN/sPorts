using System.Linq;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class DbInitializer
    {
        public static void InitializeDB(SportsContext context)
        {
            context.Database.EnsureCreated();

            //TODO: Add seeded data or use NMockaroo

            //Insert data into the database if there aren't any Addresses
            if (!context.Addresses.Any())
            {
                var addresses = new Address[]
                {
                new Address{Street="Amazing Drive"},
                new Address{Street="Not so Amazing Drive"},
                };

                foreach (Address address in addresses)
                {
                    context.Addresses.Add(address);
                }

                context.SaveChanges();
            }
        }
    }
}
