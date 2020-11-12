using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
                DeleteBookingFiles(booking.BookingReferenceNo);
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
                // using (var transaction = await context.Database.BeginTransactionAsync())
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Bookings.Add(booking);
                        booking.BookingLines.ForEach(bl => context.BookingLines.Add(bl));

                        rowsAffected = await context.SaveChangesAsync();
                        //transaction.CommitAsync();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        //transaction.RollbackAsync();
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return rowsAffected;
        }

        #endregion Store booking class & associated booking lines in db

        #region Delete booking files by referenceNo

        public void DeleteBookingFiles(int bookingReferenceNo)
        {
            File.Delete($@"\{bookingReferenceNo}.pdf");
            File.Delete($@"\{bookingReferenceNo}.txt");
        }

        public async Task<IList<BookingLine>> GetBookingLines(int bookingId)
        {
            var bookings = await _context.Bookings.Include(l => l.BookingLines).ToListAsync();
            var booking = bookings.Find(b => b.BookingId == bookingId);

            return booking?.BookingLines;
        }

        #endregion Delete booking files by referenceNo
    }
}
