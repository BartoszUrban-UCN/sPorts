using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Tests.Utils
{
    public class GenerateBookingData
    {
        public static SharedDatabaseFixture Fixture { get; set; }

        //public IEnumerator<object[]> GetEnumerator()
        //{
        //    yield return new object[] { CreateBookingNoParameters() };
        //    yield return new object[] { CreateBookingWithOneSpot() };
        //    yield return new object[] { CreateBookingWithTwoSpotsInSameMarina() };
        //    yield return new object[] { CreateBookingWithTwoSpotsInSameMarina() };
        //}

        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static async Task<bool> CreateBookingNoParameters()
        {
            using (var context = Fixture.CreateContext())
            {
                IBookingService bookingService = new BookingService(context);
                return await bookingService.CreateBooking(null, null, null);
            }
        }

        public static async Task<bool> CreateBookingWithOneSpot()
        {
            DeleteBooking();
            using (var context = Fixture.CreateContext())
            {
                IBookingService bookingService = new BookingService(context);
                BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();
                Spot spot = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();

                Dictionary<DateTime[], Spot> marinaSpotStayDates = new Dictionary<DateTime[], Spot>() {
                    { new DateTime[2] { DateTime.Now, DateTime.Now.AddDays(1) }, spot }
                };

                return await bookingService.CreateBooking(boatOwner, boat, marinaSpotStayDates);
            }
        }

        public async static Task<bool> CreateBookingWithTwoSpotsInSameMarina()
        {
            DeleteBooking();
            using (var context = Fixture.CreateContext())
            {
                IBookingService bookingService = new BookingService(context);
                BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();
                Spot spot1 = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();
                Spot spot2 = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 2).FirstOrDefault();

                Dictionary<DateTime[], Spot> marinaSpotStayDates = new Dictionary<DateTime[], Spot>() {
                    { new DateTime[2] { DateTime.Now, DateTime.Now.AddDays(1) }, spot1 },
                    { new DateTime[2] { DateTime.Now.AddDays(1), DateTime.Now.AddDays(2) }, spot2 }
                };

                return await bookingService.CreateBooking(boatOwner, boat, marinaSpotStayDates);
            }
        }

        public async static Task<bool> CreateBookingWithThreeSpotsInDifferentMarinas()
        {
            DeleteBooking();
            using (var context = Fixture.CreateContext())
            {
                IBookingService bookingService = new BookingService(context);
                BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();

                Spot spot1 = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();
                Spot spot2 = context.Spots.Where(s => s.MarinaId == 2 && s.SpotNumber == 4).FirstOrDefault();
                Spot spot3 = context.Spots.Where(s => s.MarinaId == 3 && s.SpotNumber == 5).FirstOrDefault();

                Dictionary<DateTime[], Spot> marinaSpotStayDates = new Dictionary<DateTime[], Spot>() {
                    { new DateTime[2] { DateTime.Now, DateTime.Now.AddDays(1) }, spot1 },
                    { new DateTime[2] { DateTime.Now.AddDays(1), DateTime.Now.AddDays(2) }, spot2 },
                    { new DateTime[2] { DateTime.Now.AddDays(2), DateTime.Now.AddDays(3) }, spot3 }
                };

                return await bookingService.CreateBooking(boatOwner, boat, marinaSpotStayDates);
            }
        }

        public static void DeleteBooking()
        {
            using (var context = Fixture.CreateContext())
            {
                List<Booking> bookings = new List<Booking>(context.Bookings.ToList());
                if (bookings.Count > 0)
                {
                    bookings.ForEach(b => context.Bookings.Remove(b));
                    context.SaveChanges();
                }
            }
        }
    }
}
