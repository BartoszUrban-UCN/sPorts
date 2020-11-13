using System.Collections.Generic;
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
        public async Task<List<BookingLine>> GetBookingsByMarinaOwner(MarinaOwner marinaOwner)
        {
            List<BookingLine> marinaOwnerBookings = new List<BookingLine>();
            return marinaOwnerBookings;
        }
        #endregion

        #region Get unconfirmed booking lines from bookings by marina owner
        public async Task<List<BookingLine>> GetUnconfirmedBookingLines(MarinaOwner marinaOwner)
        {
            List<BookingLine> unconfirmedBookingLines = new List<BookingLine>();
            return unconfirmedBookingLines;
        }
        #endregion

        #region Confirm spot booked by boatOnwer
        public bool ConfirmSpotBooked(int bookingLineId)
        {
            return false;
        }
        #endregion

        private BookingLine GetBookingLine(int bookingLineId)
        {
            return null;
        }

        #region After time left has been spent or all booking lines have been confirmed, send mail with booking information to boat owner
        public void SendConfirmationMail()
        {

        }
        #endregion
    }
}
