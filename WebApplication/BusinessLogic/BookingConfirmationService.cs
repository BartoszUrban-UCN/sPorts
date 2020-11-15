﻿using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingConfirmationService : IBookingConfirmationService
    {
        private readonly SportsContext _context;

        public BookingConfirmationService(SportsContext context)
        {
            _context = context;
        }

        #region Get Bookings By Marina Owner, order by time left to confirm
        public async Task<List<BookingLine>> GetBookingLinesByMarinaOwner(int marinaOwnerId)
        {
            List<BookingLine> marinaOwnerBookings = new List<BookingLine>();
            return marinaOwnerBookings;
        }
        #endregion

        #region Get unconfirmed booking lines from bookings by marina owner
        public async Task<List<BookingLine>> GetUnconfirmedBookingLines(int marinaOwnerId)
        {
            List<BookingLine> unconfirmedBookingLines = new List<BookingLine>();
            return unconfirmedBookingLines;
        }
        #endregion

        #region Confirm spot booked by boatOnwer
        public async Task<bool> ConfirmSpotBooked(int bookingLineId)
        {
            return false;
        }
        #endregion

        /// <summary>
        /// After 72 hours, email with booking info is sent to boat owner's email
        /// </summary>
        #region After time left has been spent or all booking lines have been confirmed, send mail with booking information to boat owner
        public void SendConfirmationMail()
        {

        }
        #endregion
    }
}
