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
        public BookingConfirmationServiceTests(SharedDatabaseFixture fixture) => Fixture = fixture;

        public SharedDatabaseFixture Fixture { get; set; }

        [Fact]
        public async void GetBookingsByMarinOwner_Expected0_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                int expected = 0;
                IBookingConfirmationService service = new BookingConfirmationService(context);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == 3).FirstOrDefault();

                List<BookingLine> marinaOwnerBookings = await service.GetBookingLinesByMarinaOwner(marinaOwner.MarinaOwnerId);
                int actual = marinaOwnerBookings == null ? 0 : marinaOwnerBookings.Count;

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void GetUnconfirmedBookingLines_Expected0_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                //await new BookingServiceTest(context).CreateBookingWithThreeSpotsInDifferentMarinas();
                int expected = 0;
                IBookingConfirmationService service = new BookingConfirmationService(context);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == 3).FirstOrDefault();

                List<BookingLine> spotsToConfirm = await service.GetUnconfirmedBookingLines(marinaOwner.MarinaOwnerId);
                int actual = spotsToConfirm == null ? 0 : spotsToConfirm.Count;

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
            // delete created bookings
        }
    }
}
