using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using static WebApplication.BusinessLogic.EmailService;

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
            List<BookingLine> marinaOwnerBookings = new List<BookingLine>(await _context.BookingLines.ToListAsync());
            marinaOwnerBookings.ForEach(bl =>
            {
                _context.Entry(bl).Reference(bl => bl.Spot).Load();
                _context.Entry(bl).Reference(bl => bl.Booking).Load();
                _context.Entry(bl.Spot).Reference(s => s.Marina).Load();
                _context.Entry(bl.Spot.Marina).Reference(m => m.Address).Load();
                _context.Entry(bl.Spot.Marina).Reference(m => m.MarinaOwner).Load();
                _context.Entry(bl.Spot.Marina.MarinaOwner).Reference(mo => mo.Person).Load();
                _context.Entry(bl.Booking).Reference(b => b.Boat).Load();
                _context.Entry(bl.Booking.Boat).Reference(b => b.BoatOwner).Load();
                _context.Entry(bl.Booking.Boat.BoatOwner).Reference(bo => bo.Person).Load();
            }
            );

            return marinaOwnerBookings.FindAll(bl => bl.Spot.Marina.MarinaOwner.MarinaOwnerId == marinaOwnerId);
        }
        #endregion

        #region Get unconfirmed booking lines from bookings by marina owner
        public async Task<List<BookingLine>> GetUnconfirmedBookingLines(int marinaOwnerId)
        {
            List<BookingLine> unconfirmedBookingLines = await GetBookingLinesByMarinaOwner(marinaOwnerId);
            return unconfirmedBookingLines.FindAll(bl => !bl.Confirmed);
        }
        #endregion

        #region Confirm spot booked by boatOnwer
        public async Task<bool> ConfirmSpotBooked(int bookingLineId)
        {
            using (SportsContext context = _context)
            {
                BookingLine bookingLine = _context.BookingLines.Find(bookingLineId);
                bookingLine.Confirmed = true;
                var success = await _context.SaveChangesAsync() > 0;
                SendConfirmationMail(bookingLine.BookingId);
                return success;
            }
        }
        #endregion

        /// <summary>
        /// After 72 hours, email with booking info is sent to boat owner's email
        /// </summary>
        #region After time left has been spent or all booking lines have been confirmed, send mail with booking information to boat owner
        public void SendConfirmationMail(int bookingId)
        {
            Booking booking = _context.Bookings.Find(bookingId);
            _context.Entry(booking).Reference(b => b.Boat).Load();
            _context.Entry(booking.Boat).Reference(b => b.BoatOwner).Load();
            _context.Entry(booking.Boat.BoatOwner).Reference(b => b.Person).Load();
            _context.Entry(booking).Collection(b => b.BookingLines).Load();
            booking.BookingLines.ForEach(bl =>
            {
                _context.Entry(bl).Reference(bl => bl.Spot).Load();
                _context.Entry(bl.Spot).Reference(s => s.Marina).Load();
                _context.Entry(bl.Spot.Marina).Reference(m => m.Address).Load();
                _context.Entry(bl.Spot.Marina).Reference(m => m.MarinaOwner).Load();
                _context.Entry(bl.Spot.Marina.MarinaOwner).Reference(mo => mo.Person).Load();
            }
            );

            IPDFService<Booking> service = new BookingPDFService();
            service.CreatePDFFile(booking);
            SendEmail(booking.BookingReferenceNo);
        }
        #endregion
    }
}
