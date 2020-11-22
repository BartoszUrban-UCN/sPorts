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
        public BookingConfirmationServiceTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
            GenerateBookingData.Fixture = Fixture;
        }

        public SharedDatabaseFixture Fixture { get; }

        [Fact]
        public async void GetBookingsByMarinOwner_Expected2_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;

                var bookingLineService = new BookingLineService(context);
                var bookingService = new BookingService(context, bookingLineService);
                var marinaOwnerService = new MarinaOwnerService(context, bookingService);

                Marina marina = context.Marinas.Find(1);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == marina.MarinaOwnerId).FirstOrDefault();
                bool spotsCreated = await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina();

                var marinaOwnerBookings = (List<BookingLine>) await marinaOwnerService.GetBookingLines(marinaOwner.MarinaOwnerId);
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

                var bookingLineService = new BookingLineService(context);
                var bookingService = new BookingService(context, bookingLineService);
                var marinaOwnerService = new MarinaOwnerService(context, bookingService);
                
                Marina marina = context.Marinas.Find(1);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == marina.MarinaOwnerId).FirstOrDefault();
                bool spotsCreated = await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina();

                var spotsToConfirm = (List<BookingLine>) await marinaOwnerService.GetConfirmedBookingLines(marinaOwner.MarinaOwnerId);
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
                bool spotsCreated = await GenerateBookingData.CreateBookingWithOneSpot();
                bool expected = true;

                var bookingLineService = new BookingLineService(context);
                var bookingService = new BookingService(context, bookingLineService);
                var marinaOwnerService = new MarinaOwnerService(context, bookingService);
                
                var unconfirmedBookingLines = (List<BookingLine>) await marinaOwnerService.GetUnconfirmedBookingLines(1);
                bool actual = await bookingService.ConfirmSpotBooked(unconfirmedBookingLines.First().BookingLineId);

                Assert.True(spotsCreated);
                Assert.Equal(expected, actual);
            }
        }
    }
}
