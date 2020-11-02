using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Business_Logic
{
    public class BookingService
    {
        private readonly SportsContext _context;

        public BookingService(SportsContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateBooking(BoatOwner boatOwner, Boat boat, Dictionary<Marina, DateTime[]> marinaStayDates, Dictionary<Marina, double[]> marinaPrices, Dictionary<Marina, Spot> marinaSpots)
        {
            int rowsAffected = 0;
            // create booking lines based on the information from the form
            List<BookingLine> bookingLines = CreateBookingLines(boat, marinaStayDates, marinaPrices, marinaSpots);

            if (bookingLines.Count > 0)
            {
                // calculate total price
                double totalPrice = 0;
                foreach (var bookingLine in bookingLines)
                {
                    totalPrice += bookingLine.DiscountedTotalPrice;
                }

                // associate booking lines & totalPrice with newly created booking class
                Booking booking = new Booking();
                InitBooking(ref booking, bookingLines, totalPrice);

                // store booking class & booking lines in the db
                rowsAffected = await StoreBookingInDb(booking);

                // send an email to boatOwner's email
                //SendEmail(mailTo: boatOwner.Person.Email);
            }

            return rowsAffected > 0;
        }

        #region Create booking lines based on data from the form
        private List<BookingLine> CreateBookingLines(Boat boat, Dictionary<Marina, DateTime[]> marinaStayDates, Dictionary<Marina, double[]> marinaPrices, Dictionary<Marina, Spot> marinaSpots)
        {
            List<BookingLine> bookingLines = new List<BookingLine>();

            marinaSpots?.Keys.ToList().ForEach(marina =>
            {
                BookingLine bookingLine = new BookingLine
                {
                    Spot = marinaSpots[marina],
                    Boat = boat,
                    OriginalTotalPrice = marinaPrices[marina][0],
                    AppliedDiscounts = marinaPrices[marina][1],
                    DiscountedTotalPrice = marinaPrices[marina][2],
                    StartDate = marinaStayDates[marina][0],
                    EndDate = marinaStayDates[marina][1],
                };

                bookingLines.Add(bookingLine);
            });

            return bookingLines;
        }
        #endregion

        #region Create booking class with booking lines & totalPrice
        private Booking InitBooking(ref Booking booking, List<BookingLine> bookingLines, double totalPrice)
        {
            booking = new Booking
            {
                BookingLines = bookingLines,
                BookingReferenceNo = new Random().Next(1, 1000),
                TotalPrice = totalPrice,
                PaymentStatus = "Not Paid"
            };

            return booking;
        }
        #endregion

        #region Store booking class & associated booking lines in db
        private async Task<int> StoreBookingInDb(Booking booking)
        {
            int rowsAffected = 0;

            using (SportsContext context = _context)
            {
                context.Bookings.Add(booking);
                booking.BookingLines.ForEach(bl => context.BookingLines.Add(bl));

                rowsAffected = await context.SaveChangesAsync();
            }

            return rowsAffected;
        }
        #endregion

        #region Assign random marina spot based on boat's size & availability
        public static Spot AssignSpotInMarina(Marina marina, Boat boat)
        {
            List<Spot> availableSpots = marina.Spots.Where(s => s.Available).ToList();
            Spot firstValidSpot = availableSpots.Find(s => s.MaxWidth > boat.Width && s.MaxLength > boat.Length && s.MaxDepth > boat.Depth);
            return firstValidSpot;
        }
        #endregion
    }
}
