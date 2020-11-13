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
        public async void GetBookingsByMarinOwner_Expected4_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                int expected = 4;
                IBookingConfirmationService service = new BookingConfirmationService(context);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == 1).FirstOrDefault();

                List<BookingLine> marinaOwnerBookings = await service.GetBookingsByMarinaOwner(marinaOwner);
                int actual = marinaOwnerBookings.Count;

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void GetUnconfirmedBookingLines_Expected3_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                //await new BookingServiceTest(context).CreateBookingWithThreeSpotsInDifferentMarinas();
                int expected = 3;
                IBookingConfirmationService service = new BookingConfirmationService(context);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == 1).FirstOrDefault();

                List<BookingLine> spotsToConfirm = await service.GetUnconfirmedBookingLines(marinaOwner);
                int actual = spotsToConfirm.Count;

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void ConfirmSpotBooked_ExpectedTrue_Success()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;
                IBookingConfirmationService service = new BookingConfirmationService(context);
                BookingLine bookingLine = context.BookingLines.Where(bl => bl.BookingId == 1).FirstOrDefault();

                bool actual = service.ConfirmSpotBooked(bookingLine.BookingLineId);

                Assert.Equal(expected, actual);
            }
        }

        public void Dispose()
        {
            // delete created bookings
        }
    }
}
