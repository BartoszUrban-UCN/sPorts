using System;
using System.Linq;
using WebApplication.Business_Logic;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class LoginServiceTests : IClassFixture<SharedDatabaseFixture>
    {
        public LoginServiceTests(SharedDatabaseFixture fixture) => Fixture = fixture;

        public SharedDatabaseFixture Fixture { get; }

        [Fact]
        public async void CreatePersonRequiredMissing()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var service = new LoginService(context);
                    var person = new Person { FirstName = "Bartosz" };

                    // Act Assert
                    await Assert.ThrowsAnyAsync<Exception>(() => service.CreatePerson(person));
                }
            }
        }

        [Fact]
        public async void CreatePersonEmailExists()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var service = new LoginService(context);
                    var person = new Person { FirstName = "Bartosz", LastName = "Urban", Email = "valid@email.com", Password = "123456" };

                    // Act Assert
                    await Assert.ThrowsAsync<ArgumentException>(() => service.CreatePerson(person));
                }
            }
        }

        [Fact]
        public async void CreatePersonSuccess()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var service = new LoginService(context);
                    var person = new Person { FirstName = "Bartosz", LastName = "Urban", Email = "valid100@email.com", Password = "123456" };

                    // Act
                    var success = await service.CreatePerson(person);

                    // Assert
                    Assert.True(success);
                    Assert.True(context.Persons.AsQueryable().FirstOrDefault(p => p.Email.Equals(person.Email)) != null);
                }
            }
        }
    }
}
