using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.BusinessLogic;
using WebApplication.Models;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingFormServiceTests
    {
        [Fact]
        public void GetAvailableSpots_FoundOne()
        {
            // Arrange
            var service = new BookingFormService();
            List<BookingLine> bookingLines1 = new List<BookingLine>
            {
                new BookingLine{StartDate=DateTime.Now, EndDate=DateTime.Now.AddDays(2)},
                new BookingLine{StartDate=DateTime.Now.AddDays(3), EndDate=DateTime.Now.AddDays(5)},
                new BookingLine{StartDate=DateTime.Now.AddDays(6), EndDate=DateTime.Now.AddDays(9)},
            };
            List<BookingLine> bookingLines2 = new List<BookingLine>
            {
                new BookingLine{StartDate=DateTime.Now, EndDate=DateTime.Now.AddDays(2)},
                new BookingLine{StartDate=DateTime.Now.AddDays(3), EndDate=DateTime.Now.AddDays(5)},
                new BookingLine{StartDate=DateTime.Now.AddDays(6), EndDate=DateTime.Now.AddDays(9)},
            };
            List<BookingLine> bookingLines3 = new List<BookingLine>
            {
                new BookingLine{StartDate=DateTime.Now.AddDays(8), EndDate=DateTime.Now.AddDays(12)},
            };
            List<Spot> spots = new List<Spot>
            {
                new Spot{ SpotNumber = 1, MarinaId = 1, MaxDepth=10, MaxLength=20, MaxWidth=30, BookingLines=bookingLines1},
                new Spot{ SpotNumber = 2, MarinaId = 1, MaxDepth=30, MaxLength=40, MaxWidth=30, BookingLines=bookingLines2},
                new Spot{ SpotNumber = 3, MarinaId = 1, MaxDepth=30, MaxLength=30, MaxWidth=30, BookingLines=bookingLines3}
            };

            Marina marina = new Marina { Name = "Hello", Spots = spots };
            Boat boat = new Boat { Depth = 30, Length = 30, Width = 30 };
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(6);

            // Act
            var result = service.GetAvailableSpots(
                marina,
                boat,
                startDate,
                endDate);

            // Assert
            Assert.NotNull(result);
            //Assert.Single(result);
            Assert.Equal(3, result.First().SpotNumber);
        }

        [Fact]
        public void DoesSpotFitBoat_True()
        {
            // Arrange
            var service = new BookingFormService();

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
            var result = service.DoesSpotFitBoat(boat, spot);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DoesSpotFitBoat_False()
        {
            // Arrange
            var service = new BookingFormService();

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
            var result = service.DoesSpotFitBoat(boat, spot);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DateRangeIntersects_True()
        {
            // Arrange
            var service = new BookingFormService();
            DateTime aStart = DateTime.Now;
            DateTime aEnd = DateTime.Now.AddDays(3);
            DateTime bStart = DateTime.Now.AddDays(2);
            DateTime bEnd = DateTime.Now.AddDays(5);

            // Act
            var result = service.DoesDateRangeInsersect(
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
            var service = new BookingFormService();
            DateTime aStart = DateTime.Now;
            DateTime aEnd = DateTime.Now.AddDays(3);
            DateTime bStart = DateTime.Now.AddDays(4);
            DateTime bEnd = DateTime.Now.AddDays(10);

            // Act
            var result = service.DoesDateRangeInsersect(
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
            var service = new BookingFormService();

            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(3);

            // Act
            var areDatesValid = service.AreDatesValid(startDate, endDate);

            // Assert
            Assert.True(areDatesValid);
        }

        [Fact]
        public void AreDatesValid_False()
        {
            // Arrange
            var service = new BookingFormService();

            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(3);

            // Act
            var areDatesValid = service.AreDatesValid(endDate, startDate);

            // Assert
            Assert.False(areDatesValid);
        }
    }
}
