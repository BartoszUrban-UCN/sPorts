using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using static WebApplication.BusinessLogic.EmailService;

namespace WebApplication.BusinessLogic
{
    public class BookingService : IBookingService, ICRUD<Booking>
    {
        private readonly SportsContext _context;

        public BookingService(SportsContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateBooking(Booking booking)
        {
            int rowsAffected = 0;
            // get booking lines from the booking
            List<BookingLine> bookingLines = booking.BookingLines;

            if (bookingLines?.Count > 0)
            {
                // calculate total price
                double totalPrice = 0;
                foreach (var bookingLine in bookingLines)
                {
                    totalPrice += bookingLine.DiscountedTotalPrice;
                }

                // init booking
                booking.BookingReferenceNo = new Random().Next(1, 1000);
                booking.PaymentStatus = "Not Paid";
                booking.CreationDate = DateTime.Now;
                booking.TotalPrice = totalPrice;

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

        public List<BookingLine> CreateBookingLines(Dictionary<DateTime[], Spot> marinaSpotStayDates)
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

        public async Task<Booking> FindBooking(int id)
        {
            var booking = await _context.Bookings
                                        .Include(b => b.BookingLines)
                                        .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                throw new BusinessException("bookingservice", "Booking was not found.");
            }
            return booking;
        }

        public async Task<IEnumerable<BookingLine>> GetBookingLines(int id)
        {
            var booking = await FindBooking(id);
            return booking.BookingLines;
        }

        public async Task<bool> CancelBooking(int id)
        {
            var success = false;
            try
            {
                var booking = await FindBooking(id);

                foreach (var bookingLine in booking.BookingLines)
                {
                    bookingLine.Ongoing = false;
                }

                var result = _context.SaveChanges();
                success = result > 0;
            }
            catch (Exception ex) when (ex is DbUpdateException
                                    || ex is DbUpdateConcurrencyException
                                    || ex is BusinessException)
            { }
            return success;
        }

        public Task<int> Create(Booking objectToCreate)
        {
            throw new NotImplementedException();
        }

        public Task<Booking> GetSingle(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Booking>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Booking> Update(Booking objectToUpdate)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(int? id)
        {
            throw new NotImplementedException();
        }
    }
}
