using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeDb(IServiceProvider serviceProvider, SportsContext context)
        {
            SeedDb(context);
        }
        public static void SeedDb(SportsContext context)
        {
            //TODO: Remove in Release :)
            context.Database.EnsureDeleted();

            context.Database.EnsureCreated();

            //TODO: Add seed data or use NMockaroo

            //Insert data into the database if there aren't any in the specific table
            if (!context.Addresses.Any())
            {
                var addresses = new Address[]
                {
                    new Address{Street="Amazing Drive", City="Aalborg", Country="DK", State="DK"},
                    new Address{Street="Not so Amazing Drive", City="Copenhagen", Country="DK", State="DK"},
                    new Address{ City="Aalborg", Street="Amazing Drive", Country="Denmark", PostalCode="9000", State="Denmark" },
                    new Address{ City="Aarhus", Street="Not so Amazing Drive", Country="Denmark", PostalCode="9000", State="Denmark" },
                    new Address{ City="Copenhagen", Street="Not so Amazing Drive", Country="Denmark", PostalCode="9000", State="Denmark" },
                    new Address{ City="Esbjerg", Street="Not so Amazing Drive", Country="Denmark", PostalCode="9000", State="Denmark" },
                };

                context.Addresses.AddRange(addresses);
                context.SaveChanges();
            }

            if (!context.Locations.Any())
            {
                var locations = new Location[]
                {
                    new Location { Latitude = 55.724478044067006, Longitude = 12.60262191295624 },
                    new Location { Latitude = 57.058790791383466, Longitude = 9.895803630352022 },
                    new Location { Latitude = 57.05889288811736, Longitude = 9.895127713680269 },
                };

                context.Locations.AddRange(locations);
                context.SaveChanges();
            }

            if (!context.Persons.Any())
            {
                var persons = new Person[]
                {
                    new Person{FirstName="Bartosz", LastName="Urban", Email="bartosz@email.com", AddressId=1},
                    new Person{FirstName="Dragos", LastName="Ionescu", Email="dragos@email.com", AddressId=1},
                    new Person{FirstName="Peter", LastName="Boelt", Email="peter@email.com", AddressId=2},
                    new Person{FirstName="Zach", LastName="Horatau", Email="zaharia@email.com", AddressId=2}
                };

                context.Persons.AddRange(persons);
                context.SaveChanges();
            }

            if (!context.MarinaOwners.Any())
            {
                var marinaOwners = new MarinaOwner[]
                {
                    new MarinaOwner{ PersonId=1 },
                    new MarinaOwner{ PersonId=2 },
                    new MarinaOwner{ PersonId=3 },
                    new MarinaOwner{ PersonId=4 },
                };

                context.MarinaOwners.AddRange(marinaOwners);
                context.SaveChanges();
            }

            if (!context.Marinas.Any())
            {
                var marinas = new Marina[]
                {
                    new Marina{ Name="Aalborg Marina", MarinaOwnerId=1, Description="Coolest marina", Facilities="none", AddressId=1 },
                    new Marina{ Name="Aarhus Marina", MarinaOwnerId=2, Description="Some marina", Facilities="none", AddressId=2 },
                    new Marina{ Name="Copenhagen Marina", MarinaOwnerId=3, Description="Some marina", Facilities="none", AddressId=3 },
                    new Marina{ Name="Esbjerg Marina", MarinaOwnerId=1, Description="Some marina", Facilities="none", AddressId=4 },
                };

                context.Marinas.AddRange(marinas);
                context.SaveChanges();
            }

            if (!context.BoatOwners.Any())
            {
                var boatOwners = new BoatOwner[]
                {
                    new BoatOwner{PersonId=3},
                    new BoatOwner{PersonId=4}
                };

                context.BoatOwners.AddRange(boatOwners);
                context.SaveChanges();
            }

            if (!context.Boats.Any())
            {
                var boats = new Boat[]
                {
                    new Boat{Name="Titanic", BoatOwnerId=1, Depth=2, Length=5, Width=3},
                    new Boat{Name="Mama Destroyer", BoatOwnerId=2, Depth=5, Length=20, Width=9},
                };

                context.Boats.AddRange(boats);
                context.SaveChanges();
            }

            if (!context.Spots.Any())
            {
                var spots = new Spot[]
                {
                    new Spot{ SpotNumber = 1, MarinaId = 1, Available = true, Price = 50.00, MaxDepth=30, MaxWidth=10, MaxLength=30},
                    new Spot{ SpotNumber = 2, MarinaId = 1, Available = true, Price = 50.00, MaxDepth=30, MaxWidth=30, MaxLength=30},
                    new Spot{ SpotNumber = 3, MarinaId = 1, Available = true, Price = 50.00, MaxDepth=30, MaxWidth=30, MaxLength=30},
                    new Spot{ SpotNumber = 4, MarinaId = 2, Available = true, Price = 50.00, MaxDepth=30, MaxWidth=30, MaxLength=30, LocationId = 3},
                    new Spot{ SpotNumber = 5, MarinaId = 3, Available = true, Price = 50.00, MaxDepth=30, MaxWidth=30, MaxLength=30},
                    new Spot{ SpotNumber = 1, MarinaId = 4, Available = true, Price = 50.00, MaxDepth=30, MaxWidth=30, MaxLength=30, LocationId = 1 },
                    new Spot{ SpotNumber = 2, MarinaId = 4, Available = true, Price = 40.00, MaxDepth=30, MaxWidth=30, MaxLength=30, LocationId = 2 },
                    new Spot{ SpotNumber = 3, MarinaId = 4, Available = true, Price = 30.00, MaxDepth=30, MaxWidth=30, MaxLength=30}
                };

                context.Spots.AddRange(spots);
                context.SaveChanges();
            }

            if (!context.Bookings.Any())
            {
                var bookings = new Booking[]
                {
                    new Booking {BookingReferenceNo = 1, BoatId = 1, TotalPrice = 2, PaymentStatus = "not paid", CreationDate=DateTime.Now},
                    new Booking {BookingReferenceNo = 2, BoatId = 1, TotalPrice = 10, PaymentStatus = "not paid", CreationDate=DateTime.Now},
                    new Booking {BookingReferenceNo = 3, BoatId = 1, TotalPrice = 100, PaymentStatus = "not paid", CreationDate=DateTime.Now},
                    new Booking {BookingReferenceNo = 4, BoatId = 2, TotalPrice = 83, PaymentStatus = "not paid", CreationDate=DateTime.Now},
                    new Booking {BookingReferenceNo = 5, BoatId = 2, TotalPrice = 97, PaymentStatus = "not paid", CreationDate=DateTime.Now},
                    new Booking {BookingReferenceNo = 6, BoatId = 2, TotalPrice = 23, PaymentStatus = "not paid", CreationDate=DateTime.Now}
                };

                context.Bookings.AddRange(bookings);
                context.SaveChanges();
            }

            if (!context.BookingLines.Any())
            {
                var bookingLines = new BookingLine[]
                {
                    // Ongoing: True
                    new BookingLine {Ongoing = true, BookingId = 1, SpotId= 1, DiscountedTotalPrice = 1, Confirmed = true, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1)},
                    new BookingLine {Ongoing = false, BookingId = 1, SpotId= 2, DiscountedTotalPrice = 2, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1)},
                    new BookingLine {Ongoing = false, BookingId = 1, SpotId= 3, DiscountedTotalPrice = 3,  StartDate = DateTime.Now.AddDays(3), EndDate = DateTime.Now.AddDays(4)},
                    // Ongoing: False
                    new BookingLine {Ongoing = false, BookingId = 2, SpotId= 1, DiscountedTotalPrice = 4, StartDate = DateTime.Now.AddDays(3), EndDate = DateTime.Now.AddDays(4)},
                    new BookingLine {Ongoing = false, BookingId = 2, SpotId= 2, DiscountedTotalPrice = 5, StartDate = DateTime.Now.AddDays(3), EndDate = DateTime.Now.AddDays(4)},
                    new BookingLine {Ongoing = false, BookingId = 2, SpotId= 3, DiscountedTotalPrice = 6, StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(7)},
                    // Ongoing: False
                    new BookingLine {Ongoing = false, BookingId = 3, SpotId= 1, DiscountedTotalPrice = 10, StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(7)},
                    new BookingLine {Ongoing = false, BookingId = 3, SpotId= 2, DiscountedTotalPrice = 10, StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(7)},
                    new BookingLine {Ongoing = false, BookingId = 3, SpotId= 3, DiscountedTotalPrice = 10, StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(7)},

                    // Ongoing: False
                    new BookingLine {Ongoing = false, BookingId = 4, SpotId= 1, DiscountedTotalPrice = 19, StartDate = DateTime.Now.AddDays(12), EndDate = DateTime.Now.AddDays(15)},
                    new BookingLine {Ongoing = false, BookingId = 4, SpotId= 2, DiscountedTotalPrice = 11, StartDate = DateTime.Now.AddDays(12), EndDate = DateTime.Now.AddDays(15)},
                    new BookingLine {Ongoing = false, BookingId = 4, SpotId= 3, DiscountedTotalPrice = 17, StartDate = DateTime.Now.AddDays(12), EndDate = DateTime.Now.AddDays(15)},
                    // Ongoing: True
                    new BookingLine {Ongoing = true, BookingId = 5, SpotId= 1, DiscountedTotalPrice = 15, StartDate = DateTime.Now.AddDays(22), EndDate = DateTime.Now.AddDays(25)},
                    new BookingLine {Ongoing = true, BookingId = 5, SpotId= 3, DiscountedTotalPrice = 16, StartDate = DateTime.Now.AddDays(22), EndDate = DateTime.Now.AddDays(25)},
                    new BookingLine {Ongoing = true, BookingId = 5, SpotId= 2, DiscountedTotalPrice = 14, StartDate = DateTime.Now.AddDays(22), EndDate = DateTime.Now.AddDays(25)},
                    // Ongoing: True
                    new BookingLine {Ongoing = false, BookingId = 6, SpotId= 1, DiscountedTotalPrice = 12},
                    new BookingLine {Ongoing = true, BookingId = 6, SpotId= 2, DiscountedTotalPrice = 13},
                    new BookingLine {Ongoing = true, BookingId = 6, SpotId= 5, DiscountedTotalPrice = 14}
                };

                context.BookingLines.AddRange(bookingLines);
                context.SaveChanges();
            }
        }

        public static async Task<int> EnsureMarinaOwner(IServiceProvider serviceProvider,
                                                            string testUserPw, string UserEmail)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Person>>();
            var userService = serviceProvider.GetRequiredService<UserService>();

            var user = await userManager.FindByEmailAsync(UserEmail);
            if (user == null)
            {
                user = new Person
                {
                    Email = UserEmail,
                    EmailConfirmed = true
                };
                var marinaOwner = await userService.MakePersonMarinaOwner(user);

                await userManager.CreateAsync(user, testUserPw);
                await userManager.AddToRoleAsync(user, "MarinaOwner");
            }

            if (user == null)
                throw new Exception("Not strong password.");

            return user.Id;
        }
    }
}
