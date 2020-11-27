﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        private async Task<Booking> CreateBooking(Booking booking)
        {
            // init booking
            booking.BookingReferenceNo = new Random().Next(1, 1000);
            booking.PaymentStatus = "Not Paid";
            booking.CreationDate = DateTime.Now;

            return booking;
        }

        #region Create booking line based on data from the form
        public Booking CreateBookingLine(Booking booking, DateTime startDate, DateTime endDate, Spot spot)
        {
            BookingLine bookingLine = new BookingLine
            {
                //delete this
                BookingLineId = spot.SpotId,
                // delete this
                SpotId = spot.SpotId,
                StartDate = startDate,
                EndDate = endDate,
            };

            bookingLine.Confirmed = false;
            bookingLine.Ongoing = false;
            bookingLine.OriginalTotalPrice = spot.Price * bookingLine.EndDate.Subtract(bookingLine.StartDate).TotalDays;
            bookingLine.AppliedDiscounts = 0;
            bookingLine.DiscountedTotalPrice = bookingLine.OriginalTotalPrice - bookingLine.AppliedDiscounts;

            if (booking.BookingLines is null)
            {
                booking.BookingLines = new List<BookingLine>();
            }
            else
            {
                double totalPrice = BookingCalculatePrice(booking.BookingLines);
                booking.BookingLines.Add(bookingLine);
                booking.TotalPrice = totalPrice;
            }

            return booking;
        }
        #endregion Create booking lines based on data from the form

        public async Task<Booking> SaveBooking(Booking booking)
        {
            // store booking class & booking lines in the db
            await StoreBookingInDb(booking);
            await Save();

            // create pdf file with info about the booking
            // send an email to boatOwner's email
            // delete files create in CreateBookingPdfFile
            await SendConfirmationMail(booking.BookingId);

            return booking;
        }

        public async Task<Booking> LoadObjectsInBooking(Booking booking)
        {
            foreach (var bl in booking.BookingLines)
            {
                Spot spot = await _spotService.GetSingle(bl.SpotId);
                bl.Spot = spot;
            }

            return booking;
        }

        public Dictionary<Marina, IEnumerable<BookingLine>> FilterLinesByMarina(Booking booking)
        {
            var marinaBLineDict = new Dictionary<Marina, IEnumerable<BookingLine>>();

            foreach (var bLine in booking.BookingLines)
            {
                var key = bLine.Spot.Marina;
                var value = bLine;

                if (!marinaBLineDict.ContainsKey(key))
                {
                    marinaBLineDict.Add(key: key, value: new List<BookingLine> { value });
                }
                else
                {
                    ((List<BookingLine>)marinaBLineDict[key]).Add(value);
                }
            }

            return marinaBLineDict;
        }

        public double BookingCalculatePrice(List<BookingLine> bookingLines)
        {
            bookingLines.ThrowIfNull();

            double totalPrice = 0;
            foreach (var bookingLine in bookingLines)
                totalPrice += bookingLine.DiscountedTotalPrice;

            return totalPrice;
        }

        #region Store booking class & associated booking lines in db

        private async Task StoreBookingInDb(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                booking.BookingLines.ForEach(bl => _context.BookingLines.Add(bl));

                //await Save();
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
                await SendConfirmationMail(bookingLine.BookingId);
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
            Booking booking = new Booking();
            booking = await GetSingle(bookingId);

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
        public async Task<Dictionary<int, int>> GetAllAvailableSpotsCount(IList<int> marinaIds, int boatId, DateTime startDate, DateTime endDate)
        {
            return await _bookingFormService.GetAllAvailableSpotsCount(marinaIds, boatId, startDate, endDate);
        }
        #endregion

        #region manage shoppping cart
        /// <summary>
        /// Check whether spots in the cart has not been booked by someone else
        /// If booked by someone else remove them from booking
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>Booking with valid spots</returns>
        public async Task<Booking> ValidateShoppingCart(Booking booking)
        {
            // can remove item while iterating?
            // run time periodically on a new thread?? inform user once something has changed in the booking
            IEnumerator<BookingLine> it = booking?.BookingLines.GetEnumerator();
            while (it.MoveNext())
            {
                var bookingLine = it.Current;
                List<Spot> availableSpots = new List<Spot>(await _bookingFormService.GetAvailableSpots(bookingLine.Spot.Marina.MarinaId, booking.Boat.BoatId, bookingLine.StartDate, bookingLine.EndDate));
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
