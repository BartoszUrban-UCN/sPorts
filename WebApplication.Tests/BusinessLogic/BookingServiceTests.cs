using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.BusinessLogic;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;
using Assert = Xunit.Assert;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingServiceTests : IClassFixture<SharedDatabaseFixture>
    {
        private readonly IMarinaOwnerService _marinaOwnerService;
        private readonly IBookingService _bookingService;
        private readonly GenerateBookingData _generateBookingData;
        public SharedDatabaseFixture Fixture { get; set; }

        public BookingServiceTests(SharedDatabaseFixture fixture, IMarinaOwnerService marinaOwnerService, IBookingService bookingService)
        {
            _marinaOwnerService = marinaOwnerService;
            _bookingService = bookingService;
            _generateBookingData = new GenerateBookingData(fixture, bookingService);
            Fixture = fixture;
        }

        //[Theory]
        //[ClassData(typeof(GenerateBookingData))]
        [Fact]
        public async void CreateBooking_NoParameters_Pass()
        {
            await Assert.ThrowsAnyAsync<Exception>(() => (_generateBookingData.CreateBookingNoParameters()));
        }

        [Fact]
        public async void CreateBooking_OneSpot_Pass()
        {
            bool expected = true;

            bool actual = await _generateBookingData.CreateBookingWithOneSpot() != null;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void CreateBooking_TwoSpots_Pass()
        {
            bool expected = true;

            bool actual = await _generateBookingData.CreateBookingWithTwoSpotsInSameMarina() != null;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void CreateBooking_ThreeSpots_Pass()
        {
            bool expected = true;

            bool actual = await _generateBookingData.CreateBookingWithThreeSpotsInDifferentMarinas() != null;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void GetBookingsByMarinOwner_Expected2_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;

                Marina marina = context.Marinas.Find(1);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == marina.MarinaOwnerId).FirstOrDefault();
                bool spotsCreated = await _generateBookingData.CreateBookingWithTwoSpotsInSameMarina() != null;

                var marinaOwnerBookings = (List<BookingLine>)await _bookingService.GetBookingLines(marinaOwner.MarinaOwnerId);
                bool actual = marinaOwnerBookings == null ? false : marinaOwnerBookings.Count > 0 ? true : false;

                Assert.True(spotsCreated);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void GetUnconfirmedBookingLines_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;

                Marina marina = context.Marinas.Find(1);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == marina.MarinaOwnerId).FirstOrDefault();
                bool spotsCreated = await _generateBookingData.CreateBookingWithTwoSpotsInSameMarina() != null;

                var spotsToConfirm = (List<BookingLine>)await _marinaOwnerService.GetUnconfirmedBookingLines(marinaOwner.MarinaOwnerId);
                bool actual = spotsToConfirm == null ? false : spotsToConfirm.Count > 0 ? true : false;

                Assert.True(spotsCreated);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void ConfirmSpotBooked_ExpectedTrue_Success()
        {
            using (var context = Fixture.CreateContext())
            {
                bool spotsCreated = await _generateBookingData.CreateBookingWithOneSpot() != null;
                bool expected = true;

                var unconfirmedBookingLines = (List<BookingLine>)await _marinaOwnerService.GetUnconfirmedBookingLines(1);
                bool actual = await _bookingService.ConfirmSpotBooked(unconfirmedBookingLines.First().BookingLineId);

                Assert.True(spotsCreated);
                Assert.Equal(expected, actual);
            }
        }
    }
}
