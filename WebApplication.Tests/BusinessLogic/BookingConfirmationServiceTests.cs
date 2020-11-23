using System.Collections.Generic;
using System.Linq;
using WebApplication.BusinessLogic;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingConfirmationServiceTests : IClassFixture<SharedDatabaseFixture>
    {
        private readonly IMarinaOwnerService _marinaOwnerService;
        private readonly IBookingService _bookingService;

        public BookingConfirmationServiceTests(SharedDatabaseFixture fixture, IMarinaOwnerService marinaOwnerService, IBookingService bookingService)
        {
            Fixture = fixture;
            GenerateBookingData.Fixture = Fixture;
            _marinaOwnerService = marinaOwnerService;
            _bookingService = bookingService;
        }

        public SharedDatabaseFixture Fixture { get; }

        [Fact]
        public async void GetBookingsByMarinOwner_Expected2_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;

                Marina marina = context.Marinas.Find(1);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == marina.MarinaOwnerId).FirstOrDefault();
                bool spotsCreated = await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina() != null;

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
                bool spotsCreated = await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina() != null;

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
                bool spotsCreated = await GenerateBookingData.CreateBookingWithOneSpot() != null;
                bool expected = true;

                var unconfirmedBookingLines = (List<BookingLine>)await _marinaOwnerService.GetUnconfirmedBookingLines(1);
                bool actual = await _bookingService.ConfirmSpotBooked(unconfirmedBookingLines.First().BookingLineId);

                Assert.True(spotsCreated);
                Assert.Equal(expected, actual);
            }
        }
    }
}
