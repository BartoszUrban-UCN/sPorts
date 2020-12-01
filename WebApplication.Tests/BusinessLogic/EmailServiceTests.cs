using System;
using Xunit;
using static WebApplication.BusinessLogic.EmailService;

namespace WebApplication.Tests.BusinessLogic
{
    public class EmailServiceTests
    {
        [Fact]
        public void SendEmail_WrongPassword_Fail()
        {
            Assert.ThrowsAny<Exception>(() => (SendEmail(password: "Tester1234", bookingReference: -1)));
        }

        [Fact]
        public void SendEmail_CorrectPassword_Pass()
        {
            bool result = SendEmail(password: "Tester123", bookingReference: -1);

            Assert.True(result);
        }
    }
}
