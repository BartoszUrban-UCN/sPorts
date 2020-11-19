using System;
using System.Linq;
using WebApplication.BusinessLogic;
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
        public async void CreateRequiredMissing()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var service = new LoginService(context);
                    var person = new Person { FirstName = "Bartosz" };

                    // Act Assert
                    await Assert.ThrowsAnyAsync<Exception>(() => service.Create(person));
                }
            }
        }

        [Fact]
        public async void CreateEmailExists()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var service = new LoginService(context);
                    var person = new Person { FirstName = "Bartosz", LastName = "Urban", Email = "valid@email.com", Password = "123456" };

                    // Act Assert
                    Assert.NotEqual(0, await service.Create(person));
                    await Assert.ThrowsAsync<BusinessException>(() => service.Create(person));
                }
            }
        }

        [Fact]
        public async void CreateSuccess()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var service = new LoginService(context);
                    var person = new Person { FirstName = "Bartosz", LastName = "Urban", Email = "valid100@email.com", Password = "123456" };

                    // Act
                    var id = await service.Create(person);

                    // Assert
                    Assert.NotEqual(0, id);
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
                    await service.Create(person);
                    person = context.Persons.AsQueryable().First(p => p.Email.Equals("valid100@email.com"));

                    var boatOwner = await service.MakePersonBoatOwner(person);

                    // Assert
                    Assert.NotNull(boatOwner);
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
                    await service.Create(person);
                    person = context.Persons.AsQueryable().First(p => p.Email.Equals("valid100@email.com"));

                    var marinaOwner = await service.MakePersonMarinaOwner(person);

                    // Assert
                    Assert.NotNull(marinaOwner);
                    Assert.True(context.MarinaOwners.AsQueryable().FirstOrDefault(bO => bO.PersonId.Equals(person.PersonId)) != null);

                    await Assert.ThrowsAsync<BusinessException>(() => service.MakePersonMarinaOwner(person));
                }
            }
        }
    }
}
