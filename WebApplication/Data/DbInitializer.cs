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

                var persons = new Person[]
                {
                    new Person{FirstName="Bartosz"},
                    new Person{FirstName="Dragos"},
                };

                persons.ToList<Person>().ForEach(p => context.Persons.Add(p));
                context.SaveChanges();

                var marinaOwners = new MarinaOwner[]
                {
                    new MarinaOwner{Person=persons[0] },
                    new MarinaOwner{Person=persons[1] },
                };

                marinaOwners.ToList<MarinaOwner>().ForEach(mO => context.MarinaOwners.Add(mO));
                context.SaveChanges();

                var marinas1 = new Marina[]
                {
                    new Marina{Name="Hello", MarinaOwnerId=0 },
                    new Marina{Name="Rocky Bay", MarinaOwnerId=0 },
                    new Marina{Name="HELLO", MarinaOwnerId=1 },
                    new Marina{Name="ROCKY BAY", MarinaOwnerId=1 }
                };

                marinas1.ToList<Marina>().ForEach(m => context.Marinas.Add(m));
                context.SaveChanges();
            }
        }
    }
}
