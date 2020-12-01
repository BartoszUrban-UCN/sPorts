using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingFormServiceTests : IClassFixture<SharedDatabaseFixture>
    {
        private readonly ITestOutputHelper output;

        public BookingFormServiceTests(SharedDatabaseFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            this.output = output;
        }

        public SharedDatabaseFixture Fixture { get; }

        [Fact]
        public async void GetAllAvailableSpotsCount()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var locationService = new LocationService(context);
                    var marinaService = new MarinaService(context, locationService);
                    var service = new BookingFormService(context, marinaService);
                    List<BookingLine> bookingLines1 = new List<BookingLine>
                    {
                        new BookingLine{StartDate=DateTime.Now, EndDate=DateTime.Now.AddDays(2), BookingId = 1},
                        new BookingLine{StartDate=DateTime.Now.AddDays(3), EndDate=DateTime.Now.AddDays(5), BookingId = 1},
                        new BookingLine{StartDate=DateTime.Now.AddDays(6), EndDate=DateTime.Now.AddDays(9), BookingId = 1},
                    };
                    List<BookingLine> bookingLines2 = new List<BookingLine>
                    {
                        new BookingLine{StartDate=DateTime.Now, EndDate=DateTime.Now.AddDays(2), BookingId = 1},
                        new BookingLine{StartDate=DateTime.Now.AddDays(3), EndDate=DateTime.Now.AddDays(5), BookingId = 1},
                        new BookingLine{StartDate=DateTime.Now.AddDays(6), EndDate=DateTime.Now.AddDays(9), BookingId = 1},
                    };
                    List<BookingLine> bookingLines3 = new List<BookingLine>
                    {
                        new BookingLine{StartDate=DateTime.Now.AddDays(8), EndDate=DateTime.Now.AddDays(12), BookingId = 1},
                    };
                    List<Spot> spots = new List<Spot>
                    {
                        new Spot{ SpotNumber = 1, MarinaId = 1, MaxDepth=10, MaxLength=20, MaxWidth=30, BookingLines=bookingLines1, Available=true},
                        new Spot{ SpotNumber = 2, MarinaId = 1, MaxDepth=30, MaxLength=40, MaxWidth=30, BookingLines=bookingLines2, Available=true},
                        new Spot{ SpotNumber = 3, MarinaId = 1, MaxDepth=30, MaxLength=30, MaxWidth=30, BookingLines=bookingLines3, Available=true}
                    };

                    List<Spot> spots2 = new List<Spot>
                    {
                        new Spot{ SpotNumber = 1, MarinaId = 1, MaxDepth=10, MaxLength=20, MaxWidth=30, BookingLines=bookingLines1, Available=true},
                        new Spot{ SpotNumber = 2, MarinaId = 1, MaxDepth=30, MaxLength=40, MaxWidth=30, BookingLines=bookingLines2, Available=true},
                        new Spot{ SpotNumber = 3, MarinaId = 1, MaxDepth=30, MaxLength=30, MaxWidth=30, BookingLines=bookingLines3, Available=true}
                    };

                    Marina marina = new Marina { Name = "Hello", Spots = spots, MarinaOwnerId = 1 };
                    Marina marina2 = new Marina { Name = "Better Marina", Spots = spots2, MarinaOwnerId = 1 };

                    Boat boat = new Boat { Depth = 30, Length = 30, Width = 30, BoatOwnerId = 1 };
                    DateTime startDate = DateTime.Now;
                    DateTime endDate = DateTime.Now.AddDays(1);

                    context.AddRange(spots);
                    context.AddRange(spots2);
                    context.Add(marina);
                    context.Add(marina2);
                    context.Add(boat);

                    context.SaveChanges();

                    // Act

                    var result = await service.GetAllAvailableSpotsCount(
                        new List<int> { marina.MarinaId, marina2.MarinaId },
                        boat.BoatId,
                        startDate,
                        endDate);

                    // Assert
                    Assert.NotNull(result);
                    output.WriteLine(HelperMethods.Serialize(result));
                }
            }
        }

        [Fact]
        public async void GetAvailableSpots_FoundOne()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    // Arrange
                    var locationService = new LocationService(context);
                    var marinaService = new MarinaService(context, locationService);
                    var service = new BookingFormService(context, marinaService);
                    List<BookingLine> bookingLines1 = new List<BookingLine>
                    {
                        new BookingLine{StartDate=DateTime.Now, EndDate=DateTime.Now.AddDays(2), BookingId = 1},
                        new BookingLine{StartDate=DateTime.Now.AddDays(5), EndDate=DateTime.Now.AddDays(5), BookingId = 1},
                        new BookingLine{StartDate=DateTime.Now.AddDays(6), EndDate=DateTime.Now.AddDays(9), BookingId = 1},
                    };
                    List<BookingLine> bookingLines2 = new List<BookingLine>
                    {
                        new BookingLine{StartDate=DateTime.Now, EndDate=DateTime.Now.AddDays(2), BookingId = 1},
                        new BookingLine{StartDate=DateTime.Now.AddDays(5), EndDate=DateTime.Now.AddDays(5), BookingId = 1},
                        new BookingLine{StartDate=DateTime.Now.AddDays(6), EndDate=DateTime.Now.AddDays(9), BookingId = 1},
                    };
                    List<BookingLine> bookingLines3 = new List<BookingLine>
                    {
                        new BookingLine{StartDate=DateTime.Now.AddDays(8), EndDate=DateTime.Now.AddDays(12), BookingId = 1},
                    };
                    List<Spot> spots = new List<Spot>
                    {
                        new Spot{ SpotNumber = 1, MarinaId = 1, MaxDepth=10, MaxLength=20, MaxWidth=30, BookingLines=bookingLines1, Available=true},
                        new Spot{ SpotNumber = 2, MarinaId = 1, MaxDepth=30, MaxLength=40, MaxWidth=30, BookingLines=bookingLines2, Available=true},
                        new Spot{ SpotNumber = 3, MarinaId = 1, MaxDepth=30, MaxLength=30, MaxWidth=30, BookingLines=bookingLines3, Available=true}
                    };

                    Marina marina = new Marina { Name = "Hello", Spots = spots, MarinaOwnerId = 1 };
                    Boat boat = new Boat { Depth = 1, Length = 2, Width = 3, BoatOwnerId = 1 };
                    DateTime startDate = DateTime.Now.AddDays(3);
                    DateTime endDate = DateTime.Now.AddDays(4);

                    context.AddRange(spots);
                    context.Add(marina);
                    context.Add(boat);

                    context.SaveChanges();

                    // Act
                    var result = await service.GetAvailableSpots(
                        marina.MarinaId,
                        boat.BoatId,
                        startDate,
                        endDate);

                    // Assert
                    Assert.NotNull(result);
                    //Assert.Single(result);
                    Assert.Equal(1, result.First().SpotNumber);
                }
            }
        }

        [Fact]
        public void DoesSpotFitBoat_True()
        {
            // Arrange
            Boat boat = new Boat { Depth = 30, Length = 30, Width = 30 };
            Spot spot = new Spot
            {
                SpotNumber = 3,
                MarinaId = 1,
                MaxDepth = 30,
                MaxLength = 30,
                MaxWidth = 30
            };

            // Act
            var result = HelperMethods.DoesSpotFitBoat(boat, spot);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DoesSpotFitBoat_False()
        {
            // Arrange

            Boat boat = new Boat { Depth = 30, Length = 30, Width = 30 };
            Spot spot = new Spot
            {
                SpotNumber = 3,
                MarinaId = 1,
                MaxDepth = 30,
                MaxLength = 20,
                MaxWidth = 30
            };

            // Act
            var result = HelperMethods.DoesSpotFitBoat(boat, spot);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DateRangeIntersects_True()
        {
            // Arrange
            DateTime aStart = DateTime.Now;
            DateTime aEnd = DateTime.Now.AddDays(3);
            DateTime bStart = DateTime.Now.AddDays(2);
            DateTime bEnd = DateTime.Now.AddDays(5);

            // Act
            var result = HelperMethods.AreDatesIntersecting(
                aStart,
                aEnd,
                bStart,
                bEnd);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DateRangeIntersects_False()
        {
            // Arrange
            DateTime aStart = DateTime.Now;
            DateTime aEnd = DateTime.Now.AddDays(3);
            DateTime bStart = DateTime.Now.AddDays(4);
            DateTime bEnd = DateTime.Now.AddDays(10);

            // Act
            var result = HelperMethods.AreDatesIntersecting(
                aStart,
                aEnd,
                bStart,
                bEnd);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AreDatesValid_True()
        {
            // Arrange

            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(3);

            // Act
            var areDatesValid = HelperMethods.AreDatesValid(startDate, endDate);

            // Assert
            Assert.True(areDatesValid);
        }

        [Fact]
        public void AreDatesValid_False()
        {
            // Arrange

            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(3);

            // Act
            var areDatesValid = HelperMethods.AreDatesValid(endDate, startDate);

            // Assert
            Assert.False(areDatesValid);
        }
    }
}
