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
    public class BookingService : IBookingService
    {
        private readonly SportsContext _context;

        public BookingService(SportsContext context)
        {
            _context = context;
        }

        private async Task<int> CreateBooking(Booking booking)
        {
            int rowsAffected = 0;
            // get booking lines from the booking
            List<BookingLine> bookingLines = booking?.BookingLines;

            if (bookingLines?.Count > 0)
            {
                // calculate total price
                double totalPrice = BookingCalculatePrice(bookingLines);

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

            return rowsAffected > 0 ? booking.BookingId : throw new BusinessException("Booking", "Failed to create booking.");
        }

        public double BookingCalculatePrice(List<BookingLine> bookingLines)
        {
            double totalPrice = 0;
            foreach (var bookingLine in bookingLines)
            {
                totalPrice += bookingLine.DiscountedTotalPrice;
            }

            return totalPrice;
        }

        #region Create booking lines based on data from the form
        // take List<BookingLine> as parameter
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
                };

                bookingLine.Confirmed = false;
                bookingLine.Ongoing = false;
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
                    ExplicitLoad(booking);

                }
                catch (Exception)
                {

                    throw new BusinessException("Booking", "Something went wrong when creating your booking. Please try again. If problem persists please contact our techincal service."); ;
                }
            }

            return rowsAffected;
        }

        #endregion Store booking class & associated booking lines in db

        public Booking ExplicitLoad(Booking booking)
        {
            _context.Entry(booking).Reference(b => b.Boat).Load();
            _context.Entry(booking.Boat).Reference(b => b.BoatOwner).Load();
            _context.Entry(booking.Boat.BoatOwner).Reference(b => b.Person).Load();
            _context.Entry(booking).Collection(b => b.BookingLines).Load();
            booking.BookingLines.ForEach(bl =>
            {
                _context.Entry(bl).Reference(bl => bl.Spot).Load();
                _context.Entry(bl.Spot).Reference(s => s.Location).Load();
                _context.Entry(bl.Spot).Reference(s => s.Marina).Load();
                _context.Entry(bl.Spot.Marina).Reference(m => m.Location).Load();
                _context.Entry(bl.Spot.Marina).Reference(m => m.Address).Load();
                _context.Entry(bl.Spot.Marina).Reference(m => m.MarinaOwner).Load();
                _context.Entry(bl.Spot.Marina.MarinaOwner).Reference(mo => mo.Person).Load();
            }
            );

            return booking;
        }

        public async Task<int> Create(Booking objectToCreate)
        {
            return await CreateBooking(objectToCreate);
        }

        public async Task<Booking> GetSingle(int? id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            ExplicitLoad(booking);
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetAll()
        {
            var bookings = await _context.Bookings.ToListAsync();
            bookings.ForEach(b => ExplicitLoad(b));
            return bookings;
        }

        public async Task<Booking> Update(Booking objectToUpdate)
        {
            try
            {
                _context.Bookings.Update(objectToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw new BusinessException("Booking", "Something went wrong when updating your booking. Please try again. If problem persists please contact our techincal service.");
            }
            return objectToUpdate;
        }

        public async Task Delete(int? id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw new BusinessException("Booking", "Something went wrong when deleting your booking. Please try again. If problem persists please contact our techincal service.");
            }
        }

        public async Task<bool> Exists(int? id)
        {
            return await _context.Bookings.AnyAsync(b => b.BookingId == id);
        }

        public async Task<BookingLine> GetBookingLine(int id)
        {
            IBookingLineService bookingLineService = new BookingLineService(_context);
            var bookingLine = await bookingLineService.FindBookingLine(id);
            return bookingLine;
        }

        public async Task<bool> CancelBooking(int id)
        {
            var success = false;
            try
            {
                var booking = await GetSingle(id);

                foreach (var bookingLine in booking.BookingLines)
                {
                    bookingLine.Ongoing = false;
                    _context.BookingLines.Update(bookingLine);
                }

                var result = await _context.SaveChangesAsync();
                success = result > 0;
            }
            catch (Exception ex) when (ex is DbUpdateException
                                    || ex is DbUpdateConcurrencyException
                                    || ex is BusinessException)
            {
                throw new BusinessException("Booking cancellation", "Failed to cancel your booking.");
            }
            return success;
        }

        #region IBookingConfirmationService
        public async Task<bool> ConfirmSpotBooked(int bookingLineId)
        {
            IBookingConfirmationService bookingConfirmationService = new BookingConfirmationService(_context);
            return await bookingConfirmationService.ConfirmSpotBooked(bookingLineId);
        }

        /// <summary>
        /// Gets unconfirmed booking lines by marina owner
        /// </summary>
        /// <param name="marinaOwnerId"></param>
        /// <returns>List of BookingLines</returns>
        public async Task<List<BookingLine>> GetUnconfirmedBookingLines(int marinaOwnerId)
        {
            List<BookingLine> unconfirmedBookingLines = new List<BookingLine>(await GetBookingLinesByMarinaOwner(marinaOwnerId));
            return unconfirmedBookingLines.FindAll(bl => !bl.Confirmed);
        }

        public void SendConfirmationMail(int bookingId)
        {
            IBookingConfirmationService bookingConfirmationService = new BookingConfirmationService(_context);
            bookingConfirmationService.SendConfirmationMail(bookingId);
        }
        #endregion

        #region IBookingLineService
        public async Task<List<BookingLine>> GetBookingLinesByMarinaOwner(int marinaOwnerId)
        {
            IBookingLineService bookingLineService = new BookingLineService(_context);
            return await bookingLineService.GetBookingLinesByMarinaOwner(marinaOwnerId);
        }

        public async Task<BookingLine> FindBookingLine(int id)
        {
            IBookingLineService bookingLineService = new BookingLineService(_context);
            return await bookingLineService.FindBookingLine(id);
        }

        public async Task<bool> CancelBookingLine(int id)
        {
            IBookingLineService bookingLineService = new BookingLineService(_context);
            return await bookingLineService.CancelBookingLine(id);
        }

        public async Task<bool> AddTime(int id, int amount)
        {
            IBookingLineService bookingLineService = new BookingLineService(_context);
            return await bookingLineService.AddTime(id, amount);
        }
        #endregion 
    }
}
