using System;
using WebApplication.Business_Logic;
using WebApplication.Data;
using WebApplication.Tests.Utils;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingServiceTest : SportsContextTest
    {
        [Fact]
        public async void CreateBooking_NoParameters_Fail()
        {
            using (SportsContext context = new SportsContext(ContextOptions))
            {
                BookingService bookingService = new BookingService(context);
                bool expected = true;
                bool actual = await bookingService.CreateBooking(null, null, null, null, null);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async void CreateBooking_ValidValues_Pass()
        {
            using (SportsContext context = new SportsContext(ContextOptions))
            {
                BookingService bookingService = new BookingService(context);
                bool expected = true;
                bool actual = await bookingService.CreateBooking(null, null, null, null, null);

                Assert.Equal(expected, actual);
            }
        }

        //BoatOwner boatOwner, Boat boat, Dictionary<Marina, DateTime[]> marinaStayDates, Dictionary<Marina, double[]> marinaPrices, Dictionary<Marina, Spot> marinaSpots

        [Fact]
        public void SendEmail_WrongPassword_Fail()
        {
            bool result = BookingService.SendEmail(password: "Tester1234");

            Assert.False(result);
            Assert.Throws<Exception>(() => result);
        }

        [Fact]
        public void SendEmail_CorrectPassword_Pass()
        {
            bool result = BookingService.SendEmail(password: "Tester123");

            Assert.True(result);
        }
    }
}
