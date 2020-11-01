using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Business_Logic
{
    public class BookingService
    {
        private readonly SportsContext _context;

        public BookingService(SportsContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateBooking(BoatOwner boatOwner, Boat boat, Dictionary<Marina, DateTime[]> marinaStayDates, Dictionary<Marina, double[]> marinaPrices, Dictionary<Marina, Spot> marinaSpots)
        {
            int rowsAffected = 0;
            // create booking lines based on the information from the form
            List<BookingLine> bookingLines = CreateBookingLines(boat, marinaStayDates, marinaPrices, marinaSpots);

            if (bookingLines.Count > 0)
            {
                // calculate total price
                double totalPrice = 0;
                foreach (var bookingLine in bookingLines)
                {
                    totalPrice += bookingLine.DiscountedTotalPrice;
                }

                // associate booking lines & totalPrice with newly created booking class
                Booking booking = new Booking();
                InitBooking(ref booking, bookingLines, totalPrice);

                // store booking class & booking lines in the db
                rowsAffected = await StoreBookingInDb(booking);

                // send an email to boatOwner's email
            }

            return rowsAffected > 0;
        }

        #region Create booking lines based on data from the form
        private List<BookingLine> CreateBookingLines(Boat boat, Dictionary<Marina, DateTime[]> marinaStayDates, Dictionary<Marina, double[]> marinaPrices, Dictionary<Marina, Spot> marinaSpots)
        {
            List<BookingLine> bookingLines = new List<BookingLine>();

            marinaSpots?.Keys.ToList().ForEach(marina =>
            {
                BookingLine bookingLine = new BookingLine
                {
                    Spot = marinaSpots[marina],
                    Boat = boat,
                    OriginalTotalPrice = marinaPrices[marina][0],
                    AppliedDiscounts = marinaPrices[marina][1],
                    DiscountedTotalPrice = marinaPrices[marina][2],
                    StartDate = marinaStayDates[marina][0],
                    EndDate = marinaStayDates[marina][1],
                };

                bookingLines.Add(bookingLine);
            });

            return bookingLines;
        }
        #endregion

        #region Create booking class with booking lines & totalPrice
        private Booking InitBooking(ref Booking booking, List<BookingLine> bookingLines, double totalPrice)
        {
            booking = new Booking
            {
                BookingLines = bookingLines,
                BookingReferenceNo = new Random().Next(1, 1000),
                TotalPrice = totalPrice,
                PaymentStatus = "Not Paid"
            };

            return booking;
        }
        #endregion

        #region Store booking class & associated booking lines in db
        private async Task<int> StoreBookingInDb(Booking booking)
        {
            int rowsAffected = 0;

            using (SportsContext context = _context)
            {
                context.Bookings.Add(booking);
                booking.BookingLines.ForEach(bl => context.BookingLines.Add(bl));

                rowsAffected = await context.SaveChangesAsync();
            }

            return rowsAffected;
        }
        #endregion

        public static bool SendEmail(string mailFrom = "tester6543@yandex.com", string mailTo = "tester7654@yandex.com", string username = "Tester6543", string password = "Tester123")
        {
            bool success = false;

            try
            {
                var credentials = new NetworkCredential(mailFrom, password);
                SmtpClient SmtpServer = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.yandex.com",
                    EnableSsl = true,
                    Credentials = credentials
                };

                MailMessage mail = new MailMessage(mailFrom, mailTo);
                mail.Subject = "Test Mail Attachement";
                mail.Body = "This is test mail with attachement using smtp.";

                Attachment attachment;
                attachment = new Attachment(@"c:\temp\test.txt");
                mail.Attachments.Add(attachment);

                SmtpServer.Send(mail);
                success = true;
            }
            catch (Exception)
            {
                success = false;
                throw;
            }

            return success;
        }

        #region Assign random marina spot based on boat's size & availability
        public static Spot AssignSpotInMarina(Marina marina, Boat boat)
        {
            List<Spot> availableSpots = marina.Spots.Where(s => s.Available).ToList();
            Spot firstValidSpot = availableSpots.Find(s => s.MaxWidth > boat.Width && s.MaxLength > boat.Length && s.MaxDepth > boat.Depth);
            return firstValidSpot;
        }
        #endregion
    }
}
