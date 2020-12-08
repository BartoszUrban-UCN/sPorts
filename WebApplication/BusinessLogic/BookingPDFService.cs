using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.IO;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingPDFService : IPDFService<Booking>
    {
        private static string fileFolder = Path.GetTempPath();
        /// <summary>
        /// Create pdf file with information about booking
        /// </summary>
        /// <param name="booking"></param>
        public async Task CreatePDFFile(Booking booking)
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
                                    $"Total Price: {booking.TotalPrice}\n" +
                                    "--------------------------------------------------------\n";
            booking.BookingLines.ForEach(bookingLine => bookingData += ($"Item #{bookingLine.BookingLineId}\n" +
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

            using (StreamWriter file = new StreamWriter($@"{fileFolder}\{booking.BookingReferenceNo}.txt", false))
            {
                file.WriteLine(bookingData);
            }


            //await WriteTextAsync($@"{fileFolder}\{booking.BookingReferenceNo}.txt", bookingData);

            // read from txt file & write to 
            using (StreamReader readFile = new StreamReader($@"{fileFolder}\{booking.BookingReferenceNo}.txt"))
            {
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
            }

            pdf.Save($@"{fileFolder}\{booking.BookingReferenceNo}.pdf");
            pdf.Close();
        }

        //private async Task WriteTextAsync(string filePath, string text)
        //{
        //    byte[] encodedText = Encoding.Unicode.GetBytes(text);

        //    using var sourceStream = new FileStream(
        //    filePath,
        //    FileMode.Append, FileAccess.Write, FileShare.Read,
        //    bufferSize: 4096, useAsync: true
        //    );

        //    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        //    sourceStream.Close();
        //}

        #region Delete booking files by referenceNo
        /// <summary>
        /// Deletes files from your device that were created in order to sent mail
        /// </summary>
        /// <param name="bookingReferenceNo"></param>
        public void DeleteBookingFiles(int bookingReferenceNo)
        {
            var pathPdf = $@"{fileFolder}\{bookingReferenceNo}.pdf";
            var pathTxt = $@"{fileFolder}\{bookingReferenceNo}.txt";

            try
            {
                if (File.Exists(pathPdf))
                    File.Delete(pathPdf);

                if (File.Exists(pathTxt))
                    File.Delete(pathTxt);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion Delete booking files by referenceNo
    }
}
