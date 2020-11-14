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
                    new Person{FirstName="Zach", LastName="Horatau", Email="valid4@email.com", Password="123456", AddressId=2}
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
                    new BoatOwner{PersonId=4}
                };

                context.BoatOwners.AddRange(boatOwners);
                context.SaveChanges();
            }

            if (!context.Boats.Any())
            {
                var boats = new Boat[]
                {
                    new Boat{Name="Titanic", BoatOwnerId=1},
                    new Boat{Name="Mama Destroyer", BoatOwnerId=2},
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
                    new Booking {BookingReferenceNo = 1, BoatId = 1, TotalPrice = 2},
                    new Booking {BookingReferenceNo = 2, BoatId = 1, TotalPrice = 10},
                    new Booking {BookingReferenceNo = 3, BoatId = 1, TotalPrice = 100},
                    new Booking {BookingReferenceNo = 4, BoatId = 2, TotalPrice = 83},
                    new Booking {BookingReferenceNo = 5, BoatId = 2, TotalPrice = 97},
                    new Booking {BookingReferenceNo = 6, BoatId = 2, TotalPrice = 23}
                };

                context.Bookings.AddRange(bookings);
                context.SaveChanges();
            }

            if (!context.BookingLines.Any())
            {
                var bookingLines = new BookingLine[]
                {
                    // Ongoing: True
                    new BookingLine {Ongoing = true, BookingId = 1, SpotId= 1, DiscountedTotalPrice = 1},
                    new BookingLine {Ongoing = false, BookingId = 1, SpotId= 1, DiscountedTotalPrice = 2},
                    new BookingLine {Ongoing = false, BookingId = 1, SpotId= 1, DiscountedTotalPrice = 3},
                    // Ongoing: False
                    new BookingLine {Ongoing = false, BookingId = 2, SpotId= 1, DiscountedTotalPrice = 4},
                    new BookingLine {Ongoing = false, BookingId = 2, SpotId= 1, DiscountedTotalPrice = 5},
                    new BookingLine {Ongoing = false, BookingId = 2, SpotId= 1, DiscountedTotalPrice = 6},
                    // Ongoing: False
                    new BookingLine {Ongoing = false, BookingId = 3, SpotId= 1, DiscountedTotalPrice = 10},
                    new BookingLine {Ongoing = false, BookingId = 3, SpotId= 1, DiscountedTotalPrice = 10},
                    new BookingLine {Ongoing = false, BookingId = 3, SpotId= 1, DiscountedTotalPrice = 10},
                    
                    // Ongoing: False
                    new BookingLine {Ongoing = false, BookingId = 4, SpotId= 1, DiscountedTotalPrice = 19},
                    new BookingLine {Ongoing = false, BookingId = 4, SpotId= 1, DiscountedTotalPrice = 11},
                    new BookingLine {Ongoing = false, BookingId = 4, SpotId= 1, DiscountedTotalPrice = 17},
                    // Ongoing: True
                    new BookingLine {Ongoing = true, BookingId = 5, SpotId= 1, DiscountedTotalPrice = 15},
                    new BookingLine {Ongoing = true, BookingId = 5, SpotId= 1, DiscountedTotalPrice = 16},
                    new BookingLine {Ongoing = true, BookingId = 5, SpotId= 1, DiscountedTotalPrice = 14},
                    // Ongoing: True
                    new BookingLine {Ongoing = false, BookingId = 6, SpotId= 1, DiscountedTotalPrice = 12},
                    new BookingLine {Ongoing = true, BookingId = 6, SpotId= 1, DiscountedTotalPrice = 13},
                    new BookingLine {Ongoing = true, BookingId = 6, SpotId= 1, DiscountedTotalPrice = 14}
                };

                context.BookingLines.AddRange(bookingLines);
                context.SaveChanges();
            }
        }
    }
}
