using System;
using System.Collections.Generic;
using WebApplication.BusinessLogic;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class BoatOwnerServiceTests : IClassFixture<SharedDatabaseFixture>, IDisposable
    {
        public BoatOwnerServiceTests(SharedDatabaseFixture fixture) => Fixture = fixture;

        public SharedDatabaseFixture Fixture { get; }

        [Fact]
        public void ReturnException_IfArgumentNull_ForBoat()
        {
            using (var context = Fixture.CreateContext())
            {
                // Arrange
                BoatOwner boatOwner;
                var boatOwnerService = new BoatOwnerService(context, null);
                // Act
                boatOwner = null;
                // Assert
                Assert.Throws<BusinessException>(() => boatOwnerService.GetOngoingBookings(boatOwner));
            }
        }

        [Fact]
        public void ReturnException_IfArgumentNull_ForHas()
        {
            using (var context = Fixture.CreateContext())
            {
                // Arrange
                Booking booking;
                var boatOwnerService = new BoatOwnerService(context, null);
                // Act
                booking = null;
                // Assert
                Assert.Throws<BusinessException>(() => boatOwnerService.HasOngoing(booking));
            }
        }

        [Fact]
        public void ReturnThreeOngoingBookings_WithOneBoat()
        {
            using (var context = Fixture.CreateContext())
            {
                // Arrange
                var boatOwnerService = new BoatOwnerService(context, null);

                var booking = new Booking();
                booking.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = true}
                });

                var booking1 = new Booking();
                booking1.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = true}
                });

                var booking2 = new Booking();
                booking2.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = true},
                });

                var booking3 = new Booking();
                booking3.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = false},
                });

                var boat = new Boat() { Bookings = new List<Booking> { booking, booking1, booking2, booking3 } };
                var boatOwner = new BoatOwner() { Boats = new List<Boat> { boat } };

                // Act
                var result = new List<Booking>(boatOwnerService.GetOngoingBookings(boatOwner));

                // Assert
                Assert.Collection(result, item => Assert.Equal(booking, result[0]),
                                          item => Assert.Equal(booking1, result[1]),
                                          item => Assert.Equal(booking2, result[2]));
            }
        }

        [Fact]
        public void ReturnZeroOngoingBookings_WithOneBoat()
        {
            using (var context = Fixture.CreateContext())
            {
                // Arrange
                var boatOwnerService = new BoatOwnerService(context, null);

                var booking = new Booking();
                booking.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = false}
                });

                var booking1 = new Booking();
                booking1.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = false}
                });

                var booking2 = new Booking();
                booking2.BookingLines.AddRange(new List<BookingLine>());

                var booking3 = new Booking();
                booking3.BookingLines.AddRange(new List<BookingLine>());

                var boat = new Boat() { Bookings = new List<Booking> { booking, booking1, booking2, booking3 } };
                var boatOwner = new BoatOwner() { Boats = new List<Boat> { boat } };

                // Act
                var result = new List<Booking>(boatOwnerService.GetOngoingBookings(boatOwner));

                // Assert
                Assert.True(result.Count == 0);
            }
        }

        [Fact]
        public void ReturnFourOngoingBookings_WithTwoBoats()
        {
            using (var context = Fixture.CreateContext())
            {
                // Arrange
                var boatOwnerService = new BoatOwnerService(context, null);

                var booking = new Booking();
                booking.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = true},
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = true}
                });

                var booking1 = new Booking();
                booking1.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = true},
                    new BookingLine {Ongoing = false}
                });

                var booking2 = new Booking();
                booking2.BookingLines.AddRange(new List<BookingLine>{
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = true}
                });

                var booking3 = new Booking();
                booking3.BookingLines.AddRange(new List<BookingLine>{

                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = true},
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = true}
                });

                var boat = new Boat() { Bookings = new List<Booking> { booking, booking1, } };
                var boat1 = new Boat() { Bookings = new List<Booking> { booking2, booking3 } };
                var boatOwner = new BoatOwner() { Boats = new List<Boat> { boat, boat1 } };

                // Act
                var result = new List<Booking>(boatOwnerService.GetOngoingBookings(boatOwner));

                // Assert
                Assert.Collection(result, item => Assert.Equal(booking, result[0]),
                                          item => Assert.Equal(booking1, result[1]),
                                          item => Assert.Equal(booking2, result[2]),
                                          item => Assert.Equal(booking3, result[3]));
            }
        }
        [Fact]
        public void HaveOngoing()

        {
            using (var context = Fixture.CreateContext())
            {
                // Arrange
                var boatOwnerService = new BoatOwnerService(context, null);

                var booking = new Booking();

                booking.BookingLines.AddRange(new List<BookingLine>
                {
                    new BookingLine {Ongoing = true},
                    new BookingLine {Ongoing = false},
                    new BookingLine {Ongoing = false}
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
                var boatOwnerService = new BoatOwnerService(context, null);
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
