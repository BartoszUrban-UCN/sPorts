using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;
using Assert = Xunit.Assert;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingServiceTests : IClassFixture<SharedDatabaseFixture>, IDisposable
    {
        public BookingServiceTests(SharedDatabaseFixture fixture) => Fixture = fixture;

        public SharedDatabaseFixture Fixture { get; }

        [Fact]
        public async void CreateBooking_NoParameters_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                IBookingService bookingService = new BookingService(context);
                bool expected = false;
                bool actual = await bookingService.CreateBooking(null, null, null);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void CreateBookingOneSpot_ValidValues_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;
                bool actual = await CreateBookingWithOneSpot();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void CreateBookingTwoSpots_ValidValues_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;
                bool actual = await CreateBookingWithTwoSpotsInSameMarina();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void CreateBookingThreeSpots_ValidValues_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;
                bool actual = await CreateBookingWithThreeSpotsInDifferentMarinas();

                Assert.Equal(expected, actual);
            }
        }

        public async Task<bool> CreateBookingWithOneSpot()
        {
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

        public async Task<bool> CreateBookingWithTwoSpotsInSameMarina()
        {
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

        public async Task<bool> CreateBookingWithThreeSpotsInDifferentMarinas()
        {
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

        private void DeleteBooking()
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

        public void Dispose()
        {
            DeleteBooking();
        }
    }
}
