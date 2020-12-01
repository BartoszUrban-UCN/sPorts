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
        public static SharedDatabaseFixture Fixture { get; set; }

        public GenerateBookingData(SharedDatabaseFixture fixture)
        {
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

        public static async Task<bool> CreateBookingNoParameters()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    IPDFService<Booking> pDFService = new BookingPDFService();
                    IBookingService bookingService = new BookingService(context, null, null, pDFService, null);
                    Booking booking = new Booking();
                    booking.BookingLines = new List<BookingLine>();
                    await bookingService.Create(booking);
                    return booking.BookingId < 1;
                }
            }
        }

        public static async Task<Booking> CreateBookingWithOneSpot()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    IPDFService<Booking> pDFService = new BookingPDFService();
                    IBookingService bookingService = new BookingService(context, null, null, pDFService, null);
                    BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                    Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();
                    Spot spot = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();

                    Booking booking = new Booking { Boat = boat };
                    await bookingService.Create(booking);
                    booking = bookingService.CreateBookingLine(booking, DateTime.Now, DateTime.Now.AddDays(1), spot);

                    await bookingService.SaveBooking(booking);
                    return booking;
                }
            }
        }

        public static async Task<Booking> CreateBookingWithTwoSpotsInSameMarina()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    IPDFService<Booking> pDFService = new BookingPDFService();
                    IBookingService bookingService = new BookingService(context, null, null, pDFService, null);
                    BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                    Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();
                    Spot spot1 = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();
                    Spot spot2 = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 2).FirstOrDefault();

                    Booking booking = new Booking { Boat = boat };
                    await bookingService.Create(booking);
                    booking = bookingService.CreateBookingLine(booking, DateTime.Now, DateTime.Now.AddDays(1), spot1);
                    booking = bookingService.CreateBookingLine(booking, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), spot2);

                    await bookingService.SaveBooking(booking);
                    return booking;
                }
            }
        }

        public static async Task<Booking> CreateBookingWithThreeSpotsInDifferentMarinas()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    IPDFService<Booking> pDFService = new BookingPDFService();
                    IBookingService bookingService = new BookingService(context, null, null, pDFService, null);
                    BoatOwner boatOwner = context.BoatOwners.Where(b => b.BoatOwnerId == 1).FirstOrDefault();
                    Boat boat = context.Boats.Where(b => b.BoatId == 1).FirstOrDefault();

                    Spot spot1 = context.Spots.Where(s => s.MarinaId == 1 && s.SpotNumber == 1).FirstOrDefault();
                    Spot spot2 = context.Spots.Where(s => s.MarinaId == 2 && s.SpotNumber == 4).FirstOrDefault();
                    Spot spot3 = context.Spots.Where(s => s.MarinaId == 3 && s.SpotNumber == 5).FirstOrDefault();

                    Booking booking = new Booking { Boat = boat };
                    await bookingService.Create(booking);
                    booking = bookingService.CreateBookingLine(booking, DateTime.Now, DateTime.Now.AddDays(1), spot1);
                    booking = bookingService.CreateBookingLine(booking, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), spot2);
                    booking = bookingService.CreateBookingLine(booking, DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), spot3);

                    await bookingService.SaveBooking(booking);
                    return booking;
                }
            }
        }

        public static async void DeleteBookings()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
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
}
