using System;
using System.Linq;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class DbInitializer
    {
        public static void InitializeDb(SportsContext context)
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

            if (!context.Spots.Any())
            {
                var spots = new Spot[]
                {
                    new Spot{ SpotNumber = 1, MarinaId = 1, Available = true, Price = 50.00},
                    new Spot{ SpotNumber = 2, MarinaId = 1, Available = true, Price = 50.00},
                    new Spot{ SpotNumber = 3, MarinaId = 1, Available = true, Price = 50.00},
                    new Spot{ SpotNumber = 4, MarinaId = 2, Available = true, Price = 50.00},
                    new Spot{ SpotNumber = 5, MarinaId = 3, Available = true, Price = 50.00},
                };

                context.Spots.AddRange(spots);
                context.SaveChanges();
            }

            if (!context.Bookings.Any())
            {
                var bookings = new Booking[]
                {
                    new Booking {BookingReferenceNo = 4325, TotalPrice = 12.5, PaymentStatus = "Paid", BoatId = 1, CreationDate = DateTime.Now},
                    new Booking {BookingReferenceNo = 2145, TotalPrice = 84.5, PaymentStatus = "Paid", BoatId = 2, CreationDate = DateTime.Now}
                };

                context.Bookings.AddRange(bookings);
                context.SaveChanges();
            }

            if (!context.BookingLines.Any())
            {
                var bookingLines = new BookingLine[]
                {
                    new BookingLine {OriginalTotalPrice = 5, AppliedDiscounts = 0, DiscountedTotalPrice = 5, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(3), Confirmed = true, Ongoing = true, BookingId = 1, SpotId = 1},
                    new BookingLine {OriginalTotalPrice = 5, AppliedDiscounts = 0, DiscountedTotalPrice = 5, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(3), Confirmed = true, Ongoing = true, BookingId = 1, SpotId = 1},
                    new BookingLine {OriginalTotalPrice = 2.5, AppliedDiscounts = 0, DiscountedTotalPrice = 5, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(3), Confirmed = true, Ongoing = false, BookingId = 2, SpotId = 2},
                    new BookingLine {OriginalTotalPrice = 2.5, AppliedDiscounts = 0, DiscountedTotalPrice = 5, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(3), Confirmed = true, Ongoing = true, BookingId = 2, SpotId = 2}
                };

                context.BookingLines.AddRange(bookingLines);
                context.SaveChanges();
            }
        }
    }
}
