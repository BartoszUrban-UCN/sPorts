using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Business_Logic;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingServiceTest : IClassFixture<SharedDatabaseFixture>
    {
        public BookingServiceTest(SharedDatabaseFixture fixture) => Fixture = fixture;

        public SharedDatabaseFixture Fixture { get; }

        [Fact]
        public async void CreateBooking_NoParameters_Fail()
        {
            using (var context = Fixture.CreateContext())
            {
                BookingService bookingService = new BookingService(context);
                bool expected = false;
                bool actual = await bookingService.CreateBooking(null, null, null, null, null, true);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void CreateBooking_ValidValues_Pass()
        {

            using (var context = Fixture.CreateContext())
            {
                bool expected = true;
                BookingService bookingService = new BookingService(context);
                BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();
                Marina marina = context.Marinas.Where(m => m.MarinaId == 1).FirstOrDefault();
                Spot spot = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();
                Dictionary<Marina, DateTime[]> marinaStayDates = new Dictionary<Marina, DateTime[]>() {
                    { marina, new DateTime[2] { new DateTime(), new DateTime().AddDays(1) } }
                };
                Dictionary<Marina, double[]> marinaPrices = new Dictionary<Marina, double[]>() {
                    { marina, new double[3] { 50.00, 10.00, 40.00 } }
                };
                Dictionary<Marina, Spot> marinaSpots = new Dictionary<Marina, Spot>() {
                    { marina, spot }
                };

                bool actual = await bookingService.CreateBooking(boatOwner, boat, marinaStayDates, marinaPrices, marinaSpots, true);

                Assert.Equal(expected, actual);
            }
        }
    }
}
