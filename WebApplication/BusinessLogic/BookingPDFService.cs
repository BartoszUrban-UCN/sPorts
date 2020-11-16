using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingPDFService : IPDFService<Booking>
    {
        /// <summary>
        /// Create pdf file with information about booking
        /// </summary>
        /// <param name="booking"></param>
        public void CreatePDFFile(Booking booking)
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
                                                                                $"Marina Address - {bookingLine.Spot?.Marina?.Address?.City}\n" +
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

        #region Delete booking files by referenceNo

        public void DeleteBookingFiles(int bookingReferenceNo)
        {
            File.Delete($@"\{bookingReferenceNo}.pdf");
            File.Delete($@"\{bookingReferenceNo}.txt");
        }

        #endregion Delete booking files by referenceNo
    }
}
