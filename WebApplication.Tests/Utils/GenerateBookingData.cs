using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Tests.Utils
{
    public class GenerateBookingData
    {
        private readonly IBookingService _bookingService;
        public SharedDatabaseFixture Fixture { get; set; }

        public GenerateBookingData(SharedDatabaseFixture fixture, IBookingService bookingService)
        {
            _bookingService = bookingService;
            Fixture = fixture;
        }

        //public IEnumerator<object[]> GetEnumerator()
        //{
        //    yield return new object[] { CreateBookingNoParameters() };
        //    yield return new object[] { CreateBookingWithOneSpot() };
        //    yield return new object[] { CreateBookingWithTwoSpotsInSameMarina() };
        //    yield return new object[] { CreateBookingWithTwoSpotsInSameMarina() };
        //}

        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public async Task<bool> CreateBookingNoParameters()
        {
            using (var context = Fixture.CreateContext())
            {
                //IBookingService bookingService = new BookingService(context, null)
                Booking booking = new Booking();
                booking.BookingLines = new List<BookingLine>();
                return await _bookingService.Create(booking) > 0;
            }
        }

        public async Task<Booking> CreateBookingWithOneSpot()
        {
            using (var context = Fixture.CreateContext())
            {
                //IBookingService bookingService = new BookingService(context, null);
                BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();
                Spot spot = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();

                Dictionary<DateTime[], Spot> marinaSpotStayDates = new Dictionary<DateTime[], Spot>() {
                    { new DateTime[2] { DateTime.Now, DateTime.Now.AddDays(1) }, spot }
                };
                Booking booking = new Booking { Boat = boat };
                booking.BookingLines = _bookingService.CreateBookingLines(marinaSpotStayDates);

                await _bookingService.Create(booking);
                return booking;
            }
        }

        public async Task<Booking> CreateBookingWithTwoSpotsInSameMarina()
        {
            using (var context = Fixture.CreateContext())
            {
                //IBookingService bookingService = new BookingService(context, null);
                BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();
                Spot spot1 = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();
                Spot spot2 = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 2).FirstOrDefault();

                Dictionary<DateTime[], Spot> marinaSpotStayDates = new Dictionary<DateTime[], Spot>() {
                    { new DateTime[2] { DateTime.Now, DateTime.Now.AddDays(1) }, spot1 },
                    { new DateTime[2] { DateTime.Now.AddDays(1), DateTime.Now.AddDays(2) }, spot2 }
                };
                Booking booking = new Booking { Boat = boat };
                booking.BookingLines = _bookingService.CreateBookingLines(marinaSpotStayDates);

                await _bookingService.Create(booking);
                return booking;
            }
        }

        public async Task<Booking> CreateBookingWithThreeSpotsInDifferentMarinas()
        {
            using (var context = Fixture.CreateContext())
            {
                //IBookingService bookingService = new BookingService(context, null);
                BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();

                Spot spot1 = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();
                Spot spot2 = context.Spots.Where(s => s.MarinaId == 2 && s.SpotNumber == 4).FirstOrDefault();
                Spot spot3 = context.Spots.Where(s => s.MarinaId == 3 && s.SpotNumber == 5).FirstOrDefault();

                Dictionary<DateTime[], Spot> marinaSpotStayDates = new Dictionary<DateTime[], Spot>() {
                    { new DateTime[2] { DateTime.Now, DateTime.Now.AddDays(1) }, spot1 },
                    { new DateTime[2] { DateTime.Now.AddDays(1), DateTime.Now.AddDays(2) }, spot2 },
                    { new DateTime[2] { DateTime.Now.AddDays(2), DateTime.Now.AddDays(3) }, spot3 }
                };
                Booking booking = new Booking { Boat = boat };
                booking.BookingLines = _bookingService.CreateBookingLines(marinaSpotStayDates);

                await _bookingService.Create(booking);
                return booking;
            }
        }

        public async void DeleteBookings()
        {
            using (var context = Fixture.CreateContext())
            {
                List<Booking> bookings = new List<Booking>(context.Bookings.ToList());
                if (bookings.Count > 0)
                {
                    bookings.ForEach(b => context.Bookings.Remove(b));
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
