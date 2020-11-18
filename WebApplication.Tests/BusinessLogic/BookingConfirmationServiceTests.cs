using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.BusinessLogic;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingConfirmationServiceTests : IClassFixture<SharedDatabaseFixture>, IDisposable
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
                int expected = 2;
                IBookingConfirmationService service = new BookingConfirmationService(context);
                Marina marina = context.Marinas.Find(1);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == marina.MarinaOwnerId).FirstOrDefault();
                bool spotsCreated = await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina();

                List<BookingLine> marinaOwnerBookings = await service.GetBookingLinesByMarinaOwner(marinaOwner.MarinaOwnerId);
                int actual = marinaOwnerBookings == null ? 0 : marinaOwnerBookings.Count;

                Assert.True(spotsCreated);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void GetUnconfirmedBookingLines_Expected2_Pass()
        {
            using (var context = Fixture.CreateContext())
            {

                int expected = 2;
                IBookingConfirmationService service = new BookingConfirmationService(context);
                Marina marina = context.Marinas.Find(1);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == marina.MarinaOwnerId).FirstOrDefault();
                bool spotsCreated = await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina();

                List<BookingLine> spotsToConfirm = await service.GetUnconfirmedBookingLines(marinaOwner.MarinaOwnerId);
                int actual = spotsToConfirm == null ? 0 : spotsToConfirm.Count;

                Assert.True(spotsCreated);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void ConfirmSpotBooked_ExpectedTrue_Success()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;
                IBookingConfirmationService service = new BookingConfirmationService(context);
                BookingLine bookingLine = context.BookingLines.Find(1);

                bool actual = await service.ConfirmSpotBooked(bookingLine.BookingLineId);

                Assert.Equal(expected, actual);
            }
        }

        public void Dispose()
        {
            GenerateBookingData.DeleteBooking();
        }
    }
}
