﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using static WebApplication.BusinessLogic.EmailService;

namespace WebApplication.BusinessLogic
{
    public class BookingService : IBookingService
    {
        private readonly SportsContext _context;

        public BookingService(SportsContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateBooking(BoatOwner boatOwner, Boat boat, Dictionary<DateTime[], Spot> marinaSpotStayDates)
        {
            int rowsAffected = 0;
            // create booking lines based on the information from the form
            List<BookingLine> bookingLines = CreateBookingLines(marinaSpotStayDates);

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
                InitBooking(ref booking, bookingLines, totalPrice, boat);

                // store booking class & booking lines in the db
                rowsAffected = await StoreBookingInDb(booking);

                // create pdf file with info about the booking
                IPDFService<Booking> pdfService = new BookingPDFService();
                pdfService.CreatePDFFile(booking);

                // send an email to boatOwner's email
                SendEmail(bookingReference: booking.BookingReferenceNo);

                // delete files create in CreateBookingPdfFile
                pdfService.DeleteBookingFiles(booking.BookingReferenceNo);
            }

            return rowsAffected > 0;
        }

        #region Create booking lines based on data from the form

        private List<BookingLine> CreateBookingLines(Dictionary<DateTime[], Spot> marinaSpotStayDates)
        {
            List<BookingLine> bookingLines = new List<BookingLine>();

            marinaSpotStayDates?.Keys.ToList().ForEach(date =>
            {
                BookingLine bookingLine = new BookingLine
                {
                    Spot = marinaSpotStayDates[date],
                    StartDate = date[0],
                    EndDate = date[1],
                    Confirmed = false,
                    Ongoing = false
                };

                bookingLine.OriginalTotalPrice = bookingLine.Spot.Price;
                bookingLine.AppliedDiscounts = 0;
                bookingLine.DiscountedTotalPrice = bookingLine.OriginalTotalPrice - bookingLine.AppliedDiscounts;

                bookingLines.Add(bookingLine);
            });

            return bookingLines;
        }

        #endregion Create booking lines based on data from the form

        #region Create booking class with booking lines & totalPrice

        private Booking InitBooking(ref Booking booking, List<BookingLine> bookingLines, double totalPrice, Boat boat)
        {
            booking = new Booking
            {
                BookingLines = bookingLines,
                BookingReferenceNo = new Random().Next(1, 1000),
                Boat = boat,
                TotalPrice = totalPrice,
                PaymentStatus = "Not Paid",
                CreationDate = DateTime.Now,
            };

            return booking;
        }

        #endregion Create booking class with booking lines & totalPrice

        #region Store booking class & associated booking lines in db

        private async Task<int> StoreBookingInDb(Booking booking)
        {
            int rowsAffected = 0;

            using (SportsContext context = _context)
            {
                try
                {
                    context.Bookings.Add(booking);
                    booking.BookingLines.ForEach(bl => context.BookingLines.Add(bl));

                    rowsAffected = await context.SaveChangesAsync();

                    context.Entry(booking.Boat).Reference(b => b.BoatOwner).Load();
                    context.Entry(booking.Boat.BoatOwner).Reference(b => b.Person).Load();
                    booking.BookingLines.ForEach(bl =>
                    {
                        context.Entry(bl.Spot).Reference(s => s.Marina).Load();
                        context.Entry(bl.Spot.Marina).Reference(m => m.Address).Load();
                        context.Entry(bl.Spot.Marina).Reference(m => m.MarinaOwner).Load();
                        context.Entry(bl.Spot.Marina.MarinaOwner).Reference(mo => mo.Person).Load();
                    }
                    );
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return rowsAffected;
        }

        #endregion Store booking class & associated booking lines in db

        public void AddTimeToBookingLine(BookingLine bookingLine, int seconds)
        {
            if (bookingLine == null)
            {
                throw new BusinessException("bookingservice", "The parameter can't be null.");
            }

            if (seconds < 1)
            {
                throw new BusinessException("bookingservice", "The seconds provided is invalid.");
            }

            try
            {
                bookingLine.EndDate = bookingLine.EndDate.AddSeconds(seconds);
                _context.SaveChangesAsync();
            }
            catch (Exception ex) when (ex is DbUpdateException || ex is DbUpdateConcurrencyException)
            {
                throw new BusinessException("bookingservice", "Exception when commiting to database.");
            }
        }

        public async Task<IList<BookingLine>> GetBookingLines(int bookingId)
        {
            var bookings = await _context.Bookings.Include(l => l.BookingLines).ToListAsync();
            var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                throw new BusinessException("bookingservice", "Booking was not found.");
            }

            return booking?.BookingLines;
        }
    }
}
