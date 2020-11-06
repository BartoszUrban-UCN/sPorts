﻿using System;
using System.Net;
using System.Net.Mail;

namespace WebApplication.BusinessLogic
{
    public static class EmailService
    {
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

                if (bookingReference > 0)
                {
                    Attachment attachment;
                    attachment = new Attachment($@"\{bookingReference}.pdf");
                    mail.Attachments.Add(attachment);
                    attachment = new Attachment($@"\{bookingReference}.txt");
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