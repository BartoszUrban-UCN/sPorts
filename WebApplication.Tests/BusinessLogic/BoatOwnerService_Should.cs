using System;
using System.Collections.Generic;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using WebApplication.BusinessLogic;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class BoatOwnerService_Should : IClassFixture<SharedDatabaseFixture>, IDisposable
    {
        public BoatOwnerService_Should(SharedDatabaseFixture fixture) => Fixture = fixture;

        public SharedDatabaseFixture Fixture { get; }

        public BoatOwnerService boatOwnerService { get; }

        [Fact]
        public void HaveOngoing()
        {
            using (var context = Fixture.CreateContext())
            {
                // Arrange
                var boatOwnerService = new BoatOwnerService(context);

                var booking = new Booking();

                booking.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = true},
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = true}
                });

                bool result = false;

                // Act
                result = boatOwnerService.HasOngoing(booking);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void NotHaveOngoing()
        {
            using (var context = Fixture.CreateContext())
            {
                // Arrange
                var boatOwnerService = new BoatOwnerService(context);
                var booking = new Booking();

                booking.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = false}
                });

                bool result = false;

                // Act
                result = boatOwnerService.HasOngoing(booking);

                // Assert
                Assert.False(result);
            }
        }

        public void Dispose()
        {
        }
    }
}
