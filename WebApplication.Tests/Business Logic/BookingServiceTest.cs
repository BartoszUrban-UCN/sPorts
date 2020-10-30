using System;
using System.Collections.Generic;
using WebApplication.Data;
using WebApplication.Models;
using Xunit;

namespace WebApplication.Tests.Business_Logic
{
    public class BookingServiceTest
    {
        private readonly SportsContext _context;

        public BookingServiceTest(SportsContext context)
        {
            Program.ConnectDb(Program.CreateHostBuilder(null).Build());
            _context = context;
        }

        private static Boat boat = new Boat();
        private static BoatOwner boatOwner = new BoatOwner();
        private static Marina marina = new Marina { MarinaId = 1 };

        private static Dictionary<Marina, DateTime[]> marinaStayDates = new Dictionary<Marina, DateTime[]> {
            { marina, new DateTime[2] { new DateTime(), new DateTime()} }
        };

        private static Dictionary<Marina, double[]> marinaPrices = new Dictionary<Marina, double[]> {
            {marina, new double[3] { 15, 5, 10} }
        };

        private static Dictionary<Marina, Spot> marinaSpots = new Dictionary<Marina, Spot> {
            {marina, new Spot() }
        };

        [Fact]
        public void CreateBooking_NoParameters_Fail()
        {
            bool expected = true;
            bool actual = false;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateBooking_ValidValues_Pass()
        {
            //Program.ConnectDb(Program.CreateHostBuilder(null).Build());
            //BookingService bookingService = new BookingService(new SportsContext());

            //bool expected = true;
            //bool actual = bookingService.CreateBooking(boatOwner, boat, marinaStayDates, marinaPrices, marinaSpots);

            //Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestDbConnection()
        {

        }
    }
}
