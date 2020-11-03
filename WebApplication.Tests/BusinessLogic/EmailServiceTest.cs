using System;
using Xunit;
using static WebApplication.BusinessLogic.EmailService;

namespace WebApplication.Tests.BusinessLogic
{
    public class EmailServiceTest
    {
        [Fact]
        public void SendEmail_WrongPassword_Fail()
        {
            bool result = SendEmail(password: "Tester1234", bookingReference: -1);

            Assert.ThrowsAny<Exception>(() => result);
            Assert.False(result);
        }

        [Fact]
        public void SendEmail_CorrectPassword_Pass()
        {
            bool result = SendEmail(password: "Tester123", bookingReference: -1);

            Assert.True(result);
        }
    }
}
