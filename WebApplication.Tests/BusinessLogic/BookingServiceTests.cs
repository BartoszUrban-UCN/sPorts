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
        public BookingServiceTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
            GenerateBookingData.Fixture = fixture;
        }

        public SharedDatabaseFixture Fixture { get; set; }

        //[Theory]
        //[ClassData(typeof(GenerateBookingData))]
        [Fact]
        public async void CreateBooking_NoParameters_Pass()
        {
            //await Assert.ThrowsAnyAsync<Exception>(() => (GenerateBookingData.CreateBookingNoParameters()));
            Assert.True(await GenerateBookingData.CreateBookingNoParameters());
        }

        [Fact]
        public async void CreateBooking_OneSpot_Pass()
        {
            bool expected = true;

            bool actual = (await GenerateBookingData.CreateBookingWithOneSpot()).BookingId > 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void CreateBooking_TwoSpots_Pass()
        {
            bool expected = true;

            bool actual = (await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina()).BookingId > 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void CreateBooking_ThreeSpots_Pass()
        {
            bool expected = true;

            bool actual = (await GenerateBookingData.CreateBookingWithThreeSpotsInDifferentMarinas()).BookingId > 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void GetBookingsByMarinOwner_Expected2_Pass()
        {
            using (var context = Fixture.CreateContext())
            {
                bool expected = true;

                IBookingService bookingService = new BookingService(context, null, null, null, null);
                Marina marina = context.Marinas.Find(1);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == marina.MarinaOwnerId).FirstOrDefault();
                bool spotsCreated = await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina() != null;

                var marinaOwnerBookings = (List<BookingLine>)await bookingService.GetBookingLines(marinaOwner.MarinaOwnerId);
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
                ILocationService locationService = new LocationService(context);
                IMarinaService marinaService = new MarinaService(context, locationService);
                IBookingFormService bookingFormService = new BookingFormService(context, marinaService);
                IBookingLineService service = new BookingLineService(context, bookingFormService);
                IMarinaOwnerService marinaOwnerService = new MarinaOwnerService(context, service);
                Marina marina = context.Marinas.Find(1);
                MarinaOwner marinaOwner = context.MarinaOwners.Where(mo => mo.MarinaOwnerId == marina.MarinaOwnerId).FirstOrDefault();
                bool spotsCreated = await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina() != null;

                var spotsToConfirm = (List<BookingLine>)await marinaOwnerService.GetUnconfirmedBookingLines(marinaOwner.MarinaOwnerId);
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
                // arrange
                bool spotsCreated = await GenerateBookingData.CreateBookingWithOneSpot() != null;
                bool expected = true;
                ILocationService locationService = new LocationService(context);
                IMarinaService marinaService = new MarinaService(context, locationService);
                IBookingFormService bookingFormService = new BookingFormService(context, marinaService);
                IBookingLineService service = new BookingLineService(context, bookingFormService);
                IPDFService<Booking> pDFService = new BookingPDFService();
                IBookingService bookingService = new BookingService(context, service, null, pDFService, null);
                IMarinaOwnerService marinaOwnerService = new MarinaOwnerService(context, service);

                // act
                var unconfirmedBookingLines = (List<BookingLine>)await marinaOwnerService.GetUnconfirmedBookingLines(1);
                bool actual = await bookingService.ConfirmSpotBooked(unconfirmedBookingLines.First().BookingLineId);

                // assert
                Assert.True(spotsCreated);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void CancelSpotBooked_ExpectedTrue_Success()
        {
            using (var context = Fixture.CreateContext())
            {
                bool spotsCreated = await GenerateBookingData.CreateBookingWithOneSpot() != null;
                bool expected = true;
                ILocationService locationService = new LocationService(context);
                IMarinaService marinaService = new MarinaService(context, locationService);
                IBookingFormService bookingFormService = new BookingFormService(context, marinaService);
                IBookingLineService service = new BookingLineService(context, bookingFormService);
                IPDFService<Booking> pDFService = new BookingPDFService();
                IBookingService bookingService = new BookingService(context, service, null, pDFService, null);
                IMarinaOwnerService marinaOwnerService = new MarinaOwnerService(context, service);

                var confirmedBookingLines = (List<BookingLine>)await marinaOwnerService.GetConfirmedBookingLines(1);
                bool actual = await bookingService.CancelSpotBooked(confirmedBookingLines.First().BookingLineId);

                Assert.True(spotsCreated);
                Assert.Equal(expected, actual);
            }
        }
    }
}
