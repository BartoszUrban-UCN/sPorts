using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using static WebApplication.BusinessLogic.EmailService;

namespace WebApplication.BusinessLogic
{
    public class BookingConfirmationService : ServiceBase, IBookingConfirmationService
    {
        public BookingConfirmationService(SportsContext context) : base(context)
        {
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
                bookingLine = _context.BookingLines.Find(bookingLineId);
                bookingLine.Confirmed = true;
                _context.BookingLines.Update(bookingLine);
                var success = await Save() > 0;
                SendConfirmationMail(bookingLine.BookingId);
                return success;
            }
        }
        #endregion

        /// <summary>
        /// Email with booking info is sent to boat owner's email if something has changed in the booking e.g. spot has been confirmed.
        /// </summary>
        #region After time left has been spent or all booking lines have been confirmed, send mail with booking information to boat owner
        public void SendConfirmationMail(int bookingId)
        {
            Booking booking = _context.Bookings.Find(bookingId);
            //IBookingService bookingService = new BookingService(_context);
            //bookingService.ExplicitLoad(booking);

            IPDFService<Booking> service = new BookingPDFService();
            service.CreatePDFFile(booking);
            SendEmail(bookingReference: booking.BookingReferenceNo);
            service.DeleteBookingFiles(booking.BookingReferenceNo);
        }
        #endregion
    }
}
