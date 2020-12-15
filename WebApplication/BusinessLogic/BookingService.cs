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
        private readonly ISpotService _spotService;
        private readonly IPDFService<Booking> _pdfService;

        public BookingService(SportsContext context, IBookingLineService bookingLineService, IBookingFormService bookingFormService, IPDFService<Booking> pdfService, ISpotService spotService) : base(context)
        {
            _bookingLineService = bookingLineService;
            _bookingFormService = bookingFormService;
            _pdfService = pdfService;
            _spotService = spotService;
        }

        #region Create booking line based on data from the form

        public Booking CreateBookingLine(Booking booking, DateTime startDate, DateTime endDate, Spot spot)
        {
            var bookingLine = new BookingLine
            {
                SpotId = spot.SpotId,

                StartDate = startDate,
                EndDate = endDate,

                Ongoing = default,
                Confirmed = true,
                AppliedDiscounts = default,

                OriginalTotalPrice = spot.Price * (endDate.Subtract(startDate).TotalDays + 1)
            };

            // Just for testing
            bookingLine.AppliedDiscounts = 10.5d;
            // Just for testing
            bookingLine.DiscountedTotalPrice = bookingLine.OriginalTotalPrice - bookingLine.AppliedDiscounts;

            booking.BookingLines.Add(bookingLine);
            booking.TotalPrice = CalculateTotalPrice(booking);

            return booking;
        }

        #endregion Create booking line based on data from the form

        public async Task<Booking> SaveBooking(Booking booking)
        {
            // store booking class & booking lines in the db
            await TrackBooking(booking);
            await Save();

            // create pdf file with info about the booking send an email to
            // boatOwner's email delete files create in CreateBookingPdfFile
            await SendConfirmationMail(booking.BookingId);

            return booking;
        }

        public async Task<Booking> LoadSpots(Booking booking)
        {
            booking.ThrowIfNull();

            foreach (var bookingLine in booking.BookingLines)
            {
                var spot = await _spotService.GetSingle(bookingLine.SpotId);
                bookingLine.Spot = spot;
            }

            return booking;
        }

        public Dictionary<Marina, IEnumerable<BookingLine>> FilterLinesByMarina(Booking booking)
        {
            var marinaBLineDict = new Dictionary<Marina, IEnumerable<BookingLine>>();

            foreach (var bookingLine in booking.BookingLines)
            {
                var key = bookingLine.Spot.Marina;
                var value = bookingLine;

                if (!marinaBLineDict.ContainsKey(key))
                    marinaBLineDict.Add(key: key, value: new List<BookingLine> { value });
                else
                    ((List<BookingLine>)marinaBLineDict[key]).Add(value);
            }

            return marinaBLineDict;
        }

        public double CalculateTotalDiscount(Booking booking)
        {
            booking.ThrowIfNull();

            double totalAppliedDiscounts = 0;
            foreach (var bookingLine in booking.BookingLines)
                totalAppliedDiscounts += bookingLine.AppliedDiscounts;

            totalAppliedDiscounts = Math.Round(totalAppliedDiscounts, 2);
            return totalAppliedDiscounts;
        }

        public double CalculateTotalPrice(Booking booking)
        {
            booking.ThrowIfNull();

            double totalPrice = 0;
            foreach (var bookingLine in booking.BookingLines)
                totalPrice += bookingLine.DiscountedTotalPrice;

            totalPrice = Math.Round(totalPrice, 2);
            return totalPrice;
        }

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

            booking.BookingLines.ForEach(bookingLine => bookingLine.Ongoing = false);
            _context.Update(booking);
        }

        public async Task<Booking> CreateEmptyBooking()
        {
            var booking = new Booking { };
            await Create(booking);
            booking.BookingLines.Add(new BookingLine { });
            return booking;
        }

        public async Task<int> Create(Booking booking)
        {
            booking.ThrowIfNull();

            // init booking
            booking.BookingReferenceNo = new Random().Next(1, 1000);
            booking.PaymentStatus = "Not Paid";
            booking.CreationDate = DateTime.Now;

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

        private async Task TrackBooking(Booking booking)
        {
            booking.ThrowIfNull();

            // For later use/test -Peter
            //booking.PaymentStatus = "Paid";

            await _context.AddAsync(booking);
            booking.BookingLines.ForEach(async bl => await _context.BookingLines.AddAsync(bl));
        }

        #region Confirm spot booked by boatOnwer

        /// <summary>
        /// Marina Owner confirms spot booked by a boat owner
        /// </summary>
        /// <param name="bookingLineId"></param>
        /// <returns>bool whether it has been successfully confirmed or not</returns>
        public async Task<bool> ConfirmSpotBooked(int bookingLineId)
        {
            var bookingLine = await _bookingLineService.GetSingle(bookingLineId);

            bookingLine.Confirmed = true;
            bookingLine.Ongoing = true;

            _bookingLineService.Update(bookingLine);

            var success = await Save() > 0;
            await SendConfirmationMail(bookingLine.BookingId);

            return success;
        }

        #endregion Confirm spot booked by boatOnwer

        #region Cancel / decline spot booked by boatOnwer

        /// <summary>
        /// Marina Owner cancels the spot booked by a boat owner
        /// </summary>
        /// <param name="bookingLineId"></param>
        /// <returns>bool whether it has been successfully cancelled or not</returns>
        public async Task<bool> CancelSpotBooked(int bookingLineId)
        {
            var bookingLine = await _bookingLineService.GetSingle(bookingLineId);

            bookingLine.Confirmed = false;
            bookingLine.Ongoing = true;

            _bookingLineService.Update(bookingLine);

            var success = await Save() > 0;
            await SendConfirmationMail(bookingLine.BookingId);

            return success;
        }

        #endregion Cancel / decline spot booked by boatOnwer

        #region After time left has been spent or all booking lines have been confirmed, send mail with booking information to boat owner

        /// <summary>
        /// Email with booking info is sent to boat owner's email if something
        /// has changed in the booking e.g. spot has been confirmed.
        /// </summary>
        private async Task SendConfirmationMail(int bookingId)
        {
            var booking = await GetSingle(bookingId);

            await _pdfService.CreatePDFFile(booking);
            SendEmail(bookingReference: booking.BookingReferenceNo);
            _pdfService.DeleteBookingFiles(booking.BookingReferenceNo);
        }

        #endregion After time left has been spent or all booking lines have been confirmed, send mail with booking information to boat owner

        #region IBookingLineService

        public async Task CancelBookingLine(int? id)
        {
            await _bookingLineService.CancelBookingLine(id);
        }

        public async Task<BookingLine> AddTime(int? id, int amount)
        {
            return await _bookingLineService.AddTime(id, amount);
        }
        #endregion IBookingLineService

        #region IBookingFormService

        public async Task<IEnumerable<KeyValuePair<Marina, int>>> GetAllAvailableSpotsCount(IList<int> marinaIds, int boatId, DateTime startDate, DateTime endDate)
        {
            return await _bookingFormService.GetAllAvailableSpotsCount(marinaIds, boatId, startDate, endDate);
        }

        public async Task<IList<Spot>> GetAvailableSpots(int marinaId, int boatId, DateTime startDate, DateTime endDate)
        {
            return await _bookingFormService.GetAvailableSpots(marinaId, boatId, startDate, endDate);
        }

        #endregion IBookingFormService

        #region Manage shoppping cart

        /// <summary>
        /// Check whether spots in the cart has not been booked by someone else
        /// If booked by someone else remove them from booking
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>Booking with valid spots</returns>
        public async Task<Booking> ValidateShoppingCart(Booking booking)
        {
            booking.ThrowIfNull();

            List<BookingLine> validBookingLines = new List<BookingLine>();

            foreach (var bookingLine in booking.BookingLines.ToList())
            {
                var availableSpots = (List<Spot>)await _bookingFormService.GetAvailableSpots(bookingLine.Spot.Marina.MarinaId,
                    booking.BoatId,
                    bookingLine.StartDate,
                    bookingLine.EndDate);

                if (availableSpots.Contains(bookingLine.Spot))
                    validBookingLines.Add(bookingLine);
            }

            double totalPrice = CalculateTotalPrice(booking);
            booking.TotalPrice = totalPrice;

            booking.BookingLines = new List<BookingLine>(validBookingLines);

            return booking;
        }

        public async Task<IEnumerable<BookingLine>> InvalidBookingLines(Booking booking)
        {
            booking.ThrowIfNull();

            var invalidBookingLines = new List<BookingLine>();

            foreach (var bookingLine in booking.BookingLines.ToList())
            {
                var availableSpots = (List<Spot>)await _bookingFormService.GetAvailableSpots(bookingLine.Spot.Marina.MarinaId, booking.BoatId, bookingLine.StartDate, bookingLine.EndDate);

                if (!availableSpots.Contains(bookingLine.Spot))
                    booking.BookingLines.Add(bookingLine);
            }

            return invalidBookingLines;
        }

        /// <summary>
        /// Remove booking line from the cart
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="startDate"></param>
        public Booking CartRemoveBookingLine(Booking booking, DateTime startDate)
        {
            booking.ThrowIfNull();

            booking.BookingLines.RemoveAll(bl => bl.StartDate == startDate);

            double totalPrice = CalculateTotalPrice(booking);
            booking.TotalPrice = totalPrice;
            booking.BookingLines = new List<BookingLine>(booking.BookingLines);

            return booking;
        }
        #endregion Manage shoppping cart
    }
}
