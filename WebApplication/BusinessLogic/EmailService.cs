using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace WebApplication.BusinessLogic
{
    public static class EmailService
    {
        private static string fileFolder = Path.GetTempPath();

        /// <summary>
        /// Send an email to user with booking info
        /// </summary>
        /// <param name="bookingReference"></param>
        /// <param name="mailFrom"></param>
        /// <param name="mailTo"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>bool whether the mail was sent or not</returns>
        public static bool SendEmail(int bookingReference, string mailFrom = "tester6543@yandex.com", string mailTo = "tester7654@yandex.com", string username = "Tester6543", string password = "Tester123")
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
                mail.Subject = $"Booking - {bookingReference}";
                mail.Body = "Document with information about your booking.";

                var pathPdf = $@"{fileFolder}\{bookingReference}.pdf";
                var pathTxt = $@"{ fileFolder}\{ bookingReference}.txt";
                if (bookingReference > 0 && File.Exists(pathTxt) && File.Exists(pathPdf))
                {
                    Attachment attachment;
                    attachment = new Attachment(pathTxt);
                    mail.Attachments.Add(attachment);
                    attachment = new Attachment(pathPdf);
                    mail.Attachments.Add(attachment);
                }

                SmtpServer.Send(mail);
                mail.Dispose();
                SmtpServer.Dispose();
                success = true;
            }
            catch (Exception)
            {
                throw;
            }

            return success;
        }
    }
}
