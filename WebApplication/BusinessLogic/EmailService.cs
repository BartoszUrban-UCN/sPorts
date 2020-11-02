using System;
using System.Net;
using System.Net.Mail;

namespace WebApplication.BusinessLogic
{
    public static class EmailService
    {
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

                //Attachment attachment;
                //attachment = new Attachment(@"c:\temp\test.txt");
                //mail.Attachments.Add(attachment);

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
    }
}
