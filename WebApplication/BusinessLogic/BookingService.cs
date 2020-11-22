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
        private readonly IBookingLineService _bookingLineService;

        public BookingService(SportsContext context, IBookingLineService bookingLineService)
        {
            // if (context == null)
            //     throw new BusinessException("BookingService", "The context argument was null.");

            // if (bookingLineService == null)
            //     throw new BusinessException("BookingService", "The context argument was null.");

            _context = context;
            _bookingLineService = bookingLineService;
        }

        private async Task<int> CreateBooking(Booking booking)
        {
            int rowsAffected = 0;
            // get booking lines from the booking
            var bookingLines = booking?.BookingLines;

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
                    //ExplicitLoad(booking);
                }
                catch (Exception)
                {

                    throw new BusinessException("Booking", "Something went wrong when creating your booking. Please try again. If problem persists please contact our techincal service."); ;
                }
            }

            return rowsAffected;
        }

        #endregion Store booking class & associated booking lines in db

        public async Task<IEnumerable<BookingLine>> GetBookingLines(int? id)
        {
           var booking = await GetSingle(id);
           return booking.BookingLines;
        }

        public async Task<IEnumerable<BookingLine>> GetOngoingBookingLines(int? id)
        {
            var bookingLines = await GetBookingLines(id);
            var ongoingBookingLines = new List<BookingLine>();

            foreach(var bookingLine in bookingLines)
            {
                if (bookingLine.Ongoing)
                {
                    ongoingBookingLines.Add(bookingLine);
                }
            }

            return ongoingBookingLines;
        }
        public async Task<bool> CancelBooking(int? id)
        {
           var success = false;
           try
           {
               var booking = await GetSingle(id);

               foreach (var bookingLine in booking.BookingLines)
               {
                   bookingLine.Ongoing = false;
               }

               var result = _context.SaveChanges();
               success = result > 0;
           }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException("Cancel", "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException("Cancel", "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
           return success;
        }

        public async Task<int> Create(Booking booking)
        {
            return await CreateBooking(booking);
        }

        public async Task<Booking> GetSingle(int? id)
        {
            if (id == null)
                throw new BusinessException("GetSingle", "Id is null.");
                
            if (id < 0)
                throw new BusinessException("GetSingle", "The id is negative.");

            var booking = await _context.Bookings
                                            .Include(b => b.Boat)
                                                .ThenInclude(b => b.BoatOwner)
                                                    .ThenInclude(b => b.Person)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Marina).ThenInclude(m => m.MarinaOwner).ThenInclude(m => m.Person)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Marina).ThenInclude(m => m.Location)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Marina).ThenInclude(m => m.Address)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Location)
                                            .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                throw new BusinessException("GetSingle", $"Didn't find Booking with id {id}");
            }

            return booking;
        }

        public async Task<IEnumerable<Booking>> GetAll()
        {
            var bookings = await _context.Bookings
                                            .Include(b => b.Boat)
                                                .ThenInclude(b => b.BoatOwner)
                                                    .ThenInclude(b => b.Person)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Marina).ThenInclude(m => m.MarinaOwner).ThenInclude(m => m.Person)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Marina).ThenInclude(m => m.Location)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Marina).ThenInclude(m => m.Address)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Location)
                                            .ToListAsync();
            return bookings;
        }

        public async Task<Booking> Update(Booking booking)
        {
            _context.Bookings.Update(booking);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Update", "The Booking was not updated.");

                return booking;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException("Update", "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException("Update", "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
        }

        public async Task Delete(int? id)
        {
            if (id < 0)
                throw new BusinessException("Delete", "The id argument is negative.");

            var booking = await GetSingle(id);
            _context.Bookings.Remove(booking);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Delete", "Couldn't delete the Booking.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException("Update", "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException("Update", "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
        }

        public async Task<bool> Exists(int? id)
        {
            if (id < 0)
                throw new BusinessException("Exists", "The id is negative.");

            return await _context.Bookings.AnyAsync(b => b.BookingId == id);
        }

        public async Task<BookingLine> GetBookingLine(int? id)
        {
            return await _bookingLineService.GetSingle(id);
        }

        #region IBookingConfirmationService
        public async Task<bool> ConfirmSpotBooked(int bookingLineId)
        {
            IBookingConfirmationService bookingConfirmationService = new BookingConfirmationService(_context);
            return await bookingConfirmationService.ConfirmSpotBooked(bookingLineId);
        }

        public void SendConfirmationMail(int bookingId)
        {
            IBookingConfirmationService bookingConfirmationService = new BookingConfirmationService(_context);
            bookingConfirmationService.SendConfirmationMail(bookingId);
        }
        #endregion

        #region IBookingLineService

        public async Task<bool> CancelBookingLine(int? id)
        {
            return await _bookingLineService.CancelBookingLine(id);
        }

        public async Task<bool> AddTime(int? bookingLineId, int amount)
        {
            return await _bookingLineService.AddTime(bookingLineId, amount);
        }
        #endregion
    }
}
