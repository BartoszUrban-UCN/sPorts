using System;
using System.Linq;
using WebApplication.BusinessLogic;
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
                    await Assert.ThrowsAsync<BusinessException>(() => service.CreatePerson(person));
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

        [Fact]
        public async void MakePersonBoatOwnerSuccessAndFail()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var service = new LoginService(context);
                    var person = new Person { FirstName = "Bartosz", LastName = "Urban", Email = "valid100@email.com", Password = "123456" };

                    // Act
                    await service.CreatePerson(person);
                    person = context.Persons.AsQueryable().First(p => p.Email.Equals("valid100@email.com"));

                    var success = await service.MakePersonBoatOwner(person);

                    // Assert
                    Assert.True(success);
                    Assert.True(context.BoatOwners.AsQueryable().FirstOrDefault(bO => bO.PersonId.Equals(person.PersonId)) != null);

                    await Assert.ThrowsAsync<BusinessException>(() => service.MakePersonBoatOwner(person));
                }
            }
        }

        [Fact]
        public async void MakePersonMarinaOwnerSuccessAndFail()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var service = new LoginService(context);
                    var person = new Person { FirstName = "Bartosz", LastName = "Urban", Email = "valid100@email.com", Password = "123456" };

                    // Act
                    await service.CreatePerson(person);
                    person = context.Persons.AsQueryable().First(p => p.Email.Equals("valid100@email.com"));

                    var success = await service.MakePersonMarinaOwner(person);

                    // Assert
                    Assert.True(success);
                    Assert.True(context.MarinaOwners.AsQueryable().FirstOrDefault(bO => bO.PersonId.Equals(person.PersonId)) != null);

                    await Assert.ThrowsAsync<BusinessException>(() => service.MakePersonMarinaOwner(person));
                }
            }
        }
    }
}
