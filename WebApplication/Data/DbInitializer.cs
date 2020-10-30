using System.Linq;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class DbInitializer
    {
        public static void InitializeDB(SportsContext context)
        {
            //TODO: Remove in Release :)
            //context.Database.EnsureDeleted();

            context.Database.EnsureCreated();

            //TODO: Add seed data or use NMockaroo

            //Insert data into the database if there aren't any in the specific table
            if (!context.Addresses.Any())
            {
                var addresses = new Address[]
                {
                    new Address{Street="Amazing Drive"},
                    new Address{Street="Not so Amazing Drive"},
                };

                context.Addresses.AddRange(addresses);
                context.SaveChanges();
            }

            if (!context.Persons.Any())
            {
                var persons = new Person[]
                {
                    new Person{FirstName="Bartosz", LastName="Urban", Email="valid@email.com", Password="123456", AddressId=1},
                    new Person{FirstName="Dragos", LastName="Ionescu", Email="valid2@email.com", Password="123456", AddressId=1},
                    new Person{FirstName="Peter", LastName="Boelt", Email="valid3@email.com", Password="123456", AddressId=2},
                    new Person{FirstName="Zach", LastName="Horatau", Email="valid4@email.com", Password="123456", AddressId=2},
                };

                context.Persons.AddRange(persons);
                context.SaveChanges();
            }

            if (!context.MarinaOwners.Any())
            {
                var marinaOwners = new MarinaOwner[]
                {
                    new MarinaOwner{PersonId=1 },
                    new MarinaOwner{PersonId=2 },
                };

                context.MarinaOwners.AddRange(marinaOwners);
                context.SaveChanges();
            }

            if (!context.Marinas.Any())
            {
                var marinas = new Marina[]
                {
                    new Marina{Name="Hello", MarinaOwnerId=1 },
                    new Marina{Name="Rocky Bay", MarinaOwnerId=1 },
                    new Marina{Name="HELLO", MarinaOwnerId=2 },
                    new Marina{Name="ROCKY BAY", MarinaOwnerId=2 }
                };

                context.Marinas.AddRange(marinas);
                context.SaveChanges();
            }

            if (!context.BoatOwners.Any())
            {
                var boatOwners = new BoatOwner[]
                {
                    new BoatOwner{PersonId=3},
                    new BoatOwner{PersonId=4},
                };

                context.BoatOwners.AddRange(boatOwners);
                context.SaveChanges();
            }

            if (!context.Boats.Any())
            {
                var boats = new Boat[]
                {
                    new Boat{Name="Thunderlord", BoatOwnerId=1},
                    new Boat{Name="Wave Destroyer", BoatOwnerId=2},
                };

                context.Boats.AddRange(boats);
                context.SaveChanges();
            }
        }
    }
}
