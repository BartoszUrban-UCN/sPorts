using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool CreateBooking(BoatOwner boatOwner, Boat boat, Dictionary<Marina, DateTime[]> marinaStayDates, Dictionary<Marina, double[]> marinaPrices, Dictionary<Marina, Spot> marinaSpots)
        {
            int rowsAffected = 0;

            List<BookingLine> bookingLines = new List<BookingLine>();

            marinaSpots.Keys.ToList().ForEach(marina =>
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

            double totalPrice = 0;
            foreach (var bookingLine in bookingLines)
            {
                totalPrice += bookingLine.DiscountedTotalPrice;
            }

            Booking booking = new Booking
            {
                BookingLines = bookingLines,
                BookingReferenceNo = new Random().Next(1, 1000),
                TotalPrice = totalPrice,
                PaymentStatus = "Not Paid"
            };

            using (SportsContext context = _context)
            {
                context.Bookings.Add(booking);
                booking.BookingLines.ForEach(bl => context.BookingLines.Add(bl));

                rowsAffected = context.SaveChanges();
            }

            return rowsAffected > 0;
        }

        public static Spot AssignSpotInMarina(Marina marina, Boat boat)
        {
            List<Spot> availableSpots = marina.Spots.Where(s => s.Available).ToList();
            Spot firstValidSpot = availableSpots.Find(s => s.MaxWidth > boat.Width && s.MaxLength > boat.Length && s.MaxDepth > boat.Depth);
            return firstValidSpot;
        }
    }
}
