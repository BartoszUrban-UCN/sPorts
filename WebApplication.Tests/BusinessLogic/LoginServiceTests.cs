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

                    // Act
                    await service.Create(person);

                    // Assert
                    await Assert.ThrowsAnyAsync<Exception>(() => service.Save());
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

                    // Act
                    await service.Create(person);
                    await service.Save();

                    // Assert
                    Assert.NotEqual(0, person.PersonId);

                    await Assert.ThrowsAsync<BusinessException>(async () =>
                    {
                        await service.Create(person);
                        await service.Save();
                    });
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
                    await service.Create(person);
                    await service.Save();

                    // Assert
                    Assert.NotEqual(0, person.PersonId);
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
                    await service.Save();
                    person = context.Persons.AsQueryable().First(p => p.Email.Equals("valid100@email.com"));

                    var boatOwner = await service.MakePersonBoatOwner(person);
                    await service.Save();

                    // Assert
                    Assert.NotNull(boatOwner);
                    Assert.True(context.BoatOwners.AsQueryable().FirstOrDefault(bO => bO.PersonId.Equals(person.PersonId)) != null);

                    await Assert.ThrowsAsync<BusinessException>(async () =>
                    {
                        await service.MakePersonBoatOwner(person);
                        await service.Save();
                    });
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
                    await service.Save();
                    person = context.Persons.AsQueryable().First(p => p.Email.Equals("valid100@email.com"));

                    var marinaOwner = await service.MakePersonMarinaOwner(person);
                    await service.Save();

                    // Assert
                    Assert.NotNull(marinaOwner);
                    Assert.True(context.MarinaOwners.AsQueryable().FirstOrDefault(bO => bO.PersonId.Equals(person.PersonId)) != null);

                    await Assert.ThrowsAsync<BusinessException>(async () =>
                    {
                        await service.MakePersonMarinaOwner(person);
                        await service.Save();
                    });
                }
            }
        }
    }
}
