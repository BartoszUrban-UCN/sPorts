using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Data;
using WebApplication.Models;
using static WebApplication.BusinessLogic.EmailService;

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
            List<BookingLine> bookingLines = CreateBookingLines(marinaStayDates, marinaPrices, marinaSpots);

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
                InitBooking(ref booking, bookingLines, totalPrice, boat);

                // store booking class & booking lines in the db
                rowsAffected = await StoreBookingInDb(booking);

                // create pdf file with info about the booking
                CreateBookingPdfFile(booking);

                // send an email to boatOwner's email
                SendEmail(bookingReference: booking.BookingReferenceNo);

                // delete files create in CreateBookingPdfFile
                File.Delete($@"\{booking.BookingReferenceNo}.pdf");
                File.Delete($@"\{booking.BookingReferenceNo}.txt");
            }

            return rowsAffected > 0;
        }

        #region Create booking lines based on data from the form

        private List<BookingLine> CreateBookingLines(Dictionary<Marina, DateTime[]> marinaStayDates, Dictionary<Marina, double[]> marinaPrices, Dictionary<Marina, Spot> marinaSpots)
        {
            List<BookingLine> bookingLines = new List<BookingLine>();

            marinaSpots?.Keys.ToList().ForEach(marina =>
            {
                BookingLine bookingLine = new BookingLine
                {
                    Spot = marinaSpots[marina],
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

        #endregion Create booking lines based on data from the form

        #region Create booking class with booking lines & totalPrice

        private Booking InitBooking(ref Booking booking, List<BookingLine> bookingLines, double totalPrice, Boat boat)
        {
            booking = new Booking
            {
                BookingLines = bookingLines,
                BookingReferenceNo = new Random().Next(1, 1000),
                Boat = boat,
                TotalPrice = totalPrice,
                PaymentStatus = "Not Paid"
            };

            return booking;
        }

        #endregion Create booking class with booking lines & totalPrice

        #region Store booking class & associated booking lines in db

        private async Task<int> StoreBookingInDb(Booking booking)
        {
            int rowsAffected = 0;

            using (SportsContext context = _context)
            {
                // using (var transaction = await context.Database.BeginTransactionAsync())
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Bookings.Add(booking);
                        booking.BookingLines.ForEach(bl => context.BookingLines.Add(bl));

                        rowsAffected = await context.SaveChangesAsync();
                        //transaction.CommitAsync();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        //transaction.RollbackAsync();
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return rowsAffected;
        }

        #endregion Store booking class & associated booking lines in db

        #region Create pdf file with information about booking

        private void CreateBookingPdfFile(Booking booking)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = booking.BookingReferenceNo.ToString();
            PdfPage page = pdf.AddPage();
            XGraphics graph = XGraphics.FromPdfPage(page);
            XFont fontTitle = new XFont("Verdana", 14, XFontStyle.Bold);
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);

            string bookingData = $"Booking - {booking.BookingReferenceNo}\n" +
                                    $"Boat Owner - {booking.Boat?.BoatOwner?.Person?.Email}\n" +
                                    $"Boat - {booking.Boat?.Name}\n" +
                                    $"Payment Status - {booking.PaymentStatus}\n" +
                                    $"Total Price: {booking.TotalPrice}\n";
            string bookingLinesData = "";
            booking.BookingLines.ForEach(bookingLine => bookingLinesData += ($"Item #{bookingLine.BookingLineId}\n" +
                                                                                $"Marina Owner - {bookingLine.Spot?.Marina?.MarinaOwner?.Person?.Email}\n" +
                                                                                $"Marina - {bookingLine.Spot?.Marina?.Name}\n" +
                                                                                $"Marina Address - {bookingLine.Spot?.Marina?.Address}\n" +
                                                                                $"Spot - {bookingLine.Spot?.SpotNumber}\n" +
                                                                                $"Start Date - {bookingLine.StartDate}\n" +
                                                                                $"End Date - {bookingLine.EndDate}\n" +
                                                                                $"Original Price - {bookingLine.OriginalTotalPrice}\n" +
                                                                                $"Applied Discounts - {bookingLine.AppliedDiscounts}\n" +
                                                                                $"Final Price - {bookingLine.DiscountedTotalPrice}\n" +
                                                                                $"Confirmed - {bookingLine.Confirmed}\n" +
                                                                                "--------------------------------------------------------\n"));

            using (StreamWriter file = new StreamWriter($@"\{booking.BookingReferenceNo}.txt", false))
            {
                file.WriteLine(bookingData);
                file.WriteLine(bookingLinesData);
            }

            StreamReader readFile = new StreamReader($@"\{booking.BookingReferenceNo}.txt");

            graph.DrawString($"Booking - {booking.BookingReferenceNo}", fontTitle, XBrushes.Black, new XRect(0, 20, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

            int yPoint = 50;
            string line = null;

            while (true)
            {
                line = readFile.ReadLine();
                if (line == null)
                {
                    break;
                }
                else
                {
                    graph.DrawString(line, font, XBrushes.Black, new XRect(20, yPoint, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    yPoint += 20;
                }
            }

            string pdfFileName = booking.BookingReferenceNo.ToString();
            pdf.Save($@"\{pdfFileName}.pdf");
            readFile.Close();
            pdf.Close();
        }

        #endregion Create pdf file with information about booking

        #region Assign random marina spot based on boat's size & availability

        public static Spot AssignSpotInMarina(Marina marina, Boat boat, DateTime startDate, DateTime endDate)
        {
            BookingFormService bookingFormService = new BookingFormService();
            return bookingFormService.GetFirstAvailableSpot(marina, boat, startDate, endDate);
        }

        #endregion Assign random marina spot based on boat's size & availability


    }
}