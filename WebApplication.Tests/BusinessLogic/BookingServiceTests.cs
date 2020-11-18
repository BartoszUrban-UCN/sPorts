using System;
using WebApplication.Tests.Utils;
using Xunit;
using Assert = Xunit.Assert;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingServiceTests : IClassFixture<SharedDatabaseFixture>, IDisposable
    {
        public BookingServiceTests(SharedDatabaseFixture fixture)
        {
            GenerateBookingData.Fixture = fixture;
        }


        //[Theory]
        //[ClassData(typeof(GenerateBookingData))]
        [Fact]
        public async void CreateBooking_NoParameters_Pass()
        {
            bool expected = false;

            bool actual = await GenerateBookingData.CreateBookingNoParameters();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void CreateBooking_OneSpot_Pass()
        {
            bool expected = true;

            bool actual = await GenerateBookingData.CreateBookingWithOneSpot();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void CreateBooking_TwoSpots_Pass()
        {
            bool expected = true;

            bool actual = await GenerateBookingData.CreateBookingWithTwoSpotsInSameMarina();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void CreateBooking_ThreeSpots_Pass()
        {
            bool expected = true;

            bool actual = await GenerateBookingData.CreateBookingWithThreeSpotsInDifferentMarinas();

            Assert.Equal(expected, actual);
        }

        public void Dispose()
        {
            //GenerateBookingData.DeleteBookings();
        }
    }
}
