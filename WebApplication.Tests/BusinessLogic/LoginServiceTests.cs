using System;
using WebApplication.Business_Logic;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class LoginServiceTests : SportsContextTest
    {
        [Fact]
        public void CreatePersonAlreadyExists()
        {
            using (var context = new SportsContext(ContextOptions))
            {
                // Arrange
                var service = new LoginService(context);
                var person = new Person { PersonId = 1, FirstName = "Bartosz" };

                // Act Assert
                Assert.Throws<ArgumentException>(() => service.CreatePerson(person));
            }
        }
    }
}
