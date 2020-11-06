using System;
using System.Collections.Generic;
using WebApplication.BusinessLogic;
using WebApplication.Models;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class BookingFormServiceTests
    {
        [Fact]
        public void GetFirstAvailableSpot_StateUnderTest_ExpectedBehavior()
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
            var result = service.GetFirstAvailableSpot(
                marina,
                boat,
                startDate,
                endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.SpotNumber);
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
            var result = BookingFormService.DoesSpotFitBoat(boat, spot);

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
            var result = BookingFormService.DoesSpotFitBoat(boat, spot);

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
            var result = BookingFormService.DoesDateRangeInsersect(
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
            var result = BookingFormService.DoesDateRangeInsersect(
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
            var areDatesValid = BookingFormService.AreDatesValid(startDate, endDate);

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
            var areDatesValid = BookingFormService.AreDatesValid(endDate, startDate);

            // Assert
            Assert.False(areDatesValid);
        }
    }
}
