using System;
using WebApplication.Tests.Utils;
using Xunit;
using Assert = Xunit.Assert;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingServiceTests : IClassFixture<SharedDatabaseFixture>
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
            await Assert.ThrowsAnyAsync<Exception>(() => (GenerateBookingData.CreateBookingNoParameters()));
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
    }
}
