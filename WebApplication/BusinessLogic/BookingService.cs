using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;
using static WebApplication.BusinessLogic.EmailService;

namespace WebApplication.BusinessLogic
{
    public class BookingService : ServiceBase, IBookingService
    {
        private readonly IBookingLineService _bookingLineService;
        private readonly IBookingFormService _bookingFormService;
        private readonly IPDFService<Booking> _pdfService;

        public BookingService(SportsContext context, IBookingLineService bookingLineService, IBookingFormService bookingFormService, IPDFService<Booking> pdfService) : base(context)
        {
            _bookingLineService = bookingLineService;
            _bookingFormService = bookingFormService;
            _pdfService = pdfService;
        }

        private async Task<Booking> CreateBooking(Booking booking)
        {
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
                await StoreBookingInDb(booking);

                // create pdf file with info about the booking
                // send an email to boatOwner's email
                // delete files create in CreateBookingPdfFile
                SendConfirmationMail(booking.BookingId);
            }

            return booking;
        }

        public double BookingCalculatePrice(List<BookingLine> bookingLines)
        {
            bookingLines.ThrowIfNull();

            double totalPrice = 0;
            foreach (var bookingLine in bookingLines)
                totalPrice += bookingLine.DiscountedTotalPrice;

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
                bookingLine.OriginalTotalPrice = bookingLine.Spot.Price * bookingLine.EndDate.Subtract(bookingLine.StartDate).TotalDays;
                bookingLine.AppliedDiscounts = 0;
                bookingLine.DiscountedTotalPrice = bookingLine.OriginalTotalPrice - bookingLine.AppliedDiscounts;

                bookingLines.Add(bookingLine);
            });

            return bookingLines;
        }

        #endregion Create booking lines based on data from the form

        #region Store booking class & associated booking lines in db

        private async Task StoreBookingInDb(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                booking.BookingLines.ForEach(bl => _context.BookingLines.Add(bl));

                await Save();
            }
            catch (Exception)
            {
                throw new BusinessException("Booking", "Something went wrong when creating your booking. Please try again. If problem persists please contact our techincal service."); ;
            }
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

            foreach (var bookingLine in bookingLines)
                if (bookingLine.Ongoing)
                    ongoingBookingLines.Add(bookingLine);

            return ongoingBookingLines;
        }

        public async Task CancelBooking(int? id)
        {
            var booking = await GetSingle(id);

            booking.BookingLines.ForEach(bookingLine =>
            {
                bookingLine.Ongoing = false;
                _bookingLineService.Update(bookingLine);
            });

            await Save();
        }

        public async Task<int> Create(Booking booking)
        {
            await CreateBooking(booking);

            return booking.BookingId;
        }

        public async Task<Booking> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var booking = await _context.Bookings
                                            .Include(b => b.Boat)
                                                .ThenInclude(b => b.BoatOwner)
                                                    .ThenInclude(b => b.Person)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Marina).ThenInclude(m => m.MarinaOwner).ThenInclude(m => m.Person)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Marina).ThenInclude(m => m.Location)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Marina).ThenInclude(m => m.Address)
                                            .Include(b => b.BookingLines).ThenInclude(b => b.Spot).ThenInclude(s => s.Location)
                                            .FirstOrDefaultAsync(b => b.BookingId == id);

            booking.ThrowIfNull();

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

        public Booking Update(Booking booking)
        {
            _context.Update(booking);

            return booking;
        }

        public async Task Delete(int? id)
        {
            var booking = await GetSingle(id);

            _context.Remove(booking);
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();

            return await _context.Bookings.AnyAsync(b => b.BookingId == id);
        }

        public async Task<BookingLine> GetBookingLine(int? id)
        {
            return await _bookingLineService.GetSingle(id);
        }

        #region Confirm spot booked by boatOnwer
        /// <summary>
        /// Marina Owner confirms spot booked by a boat owner
        /// </summary>
        /// <param name="bookingLineId"></param>
        /// <returns>bool whether it has been successfully confirmed or not</returns>
        public async Task<bool> ConfirmSpotBooked(int bookingLineId)
        {
            using (SportsContext context = _context)
            {
                BookingLine bookingLine = new BookingLine();
                bookingLine = await _bookingLineService.GetSingle(bookingLineId);
                bookingLine.Confirmed = true;
                _bookingLineService.Update(bookingLine);
                var success = await Save() > 0;
                SendConfirmationMail(bookingLine.BookingId);
                return success;
            }
        }
        #endregion

        #region After time left has been spent or all booking lines have been confirmed, send mail with booking information to boat owner
        /// <summary>
        /// Email with booking info is sent to boat owner's email if something has changed in the booking e.g. spot has been confirmed.
        /// </summary>
        private async Task SendConfirmationMail(int bookingId)
        {
            Booking booking = await GetSingle(bookingId);

            _pdfService.CreatePDFFile(booking);
            SendEmail(bookingReference: booking.BookingReferenceNo);
            _pdfService.DeleteBookingFiles(booking.BookingReferenceNo);
        }
        #endregion

        #region IBookingLineService

        public async Task CancelBookingLine(int? id)
        {
            await _bookingLineService.CancelBookingLine(id);
        }

        public async Task AddTime(int? bookingLineId, int amount)
        {
            await _bookingLineService.AddTime(bookingLineId, amount);
        }
        #endregion

        #region IBookingFormService
        public async Task<Dictionary<int, int>> GetAllAvailableSpotsCount(IList<int> marinaIds, string boatName, DateTime startDate, DateTime endDate)
        {
            return await _bookingFormService.GetAllAvailableSpotsCount(marinaIds, boatName, startDate, endDate);
        }
        #endregion

        #region manage shoppping cart
        /// <summary>
        /// Check whether spots in the cart has not been booked by someone else
        /// If booked by someone else remove them from booking
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>Booking with valid spots</returns>
        public Booking ValidateShoppingCart(Booking booking)
        {
            // can remove item while iterating?
            // run time periodically on a new thread?? inform user once something has changed in the booking
            IEnumerator<BookingLine> it = booking?.BookingLines.GetEnumerator();
            while (it.MoveNext())
            {
                var bookingLine = it.Current;
                List<Spot> availableSpots = new List<Spot>(_bookingFormService.GetAvailableSpots(bookingLine.Spot.Marina.MarinaId, booking.Boat.BoatId, bookingLine.StartDate, bookingLine.EndDate));
                if (!availableSpots.Contains(bookingLine.Spot))
                {
                    booking.BookingLines.Remove(bookingLine);
                }
            }

            return booking;
        }

        /// <summary>
        /// Remove booking line from the cart
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="bookingLine"></param>
        public Booking CartRemoveBookingLine(Booking booking, BookingLine bookingLine)
        {
            booking?.BookingLines.Remove(bookingLine);
            return booking;
        }
        #endregion
    }
}
