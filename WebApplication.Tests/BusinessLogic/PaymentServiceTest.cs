using WebApplication.BusinessLogic;
using WebApplication.Models;
using WebApplication.Tests.Utils;
using Xunit;

namespace WebApplication.Tests.BusinessLogic
{
    public class PaymentServiceTests : IClassFixture<SharedDatabaseFixture>
    {
        public PaymentServiceTests(SharedDatabaseFixture fixture) => Fixture = fixture;

        public SharedDatabaseFixture Fixture { get; }

        [Fact]
        public void CreatePayment()
        {
            using (var context = Fixture.CreateContext())
            {
                // Arrange

                var booking = new Booking();
                booking.TotalPrice = 100;
                var paymentService = new PaymentService(context);

                // Act
                var payment = paymentService.CreateFromBooking(booking);

                // Assert
                Assert.Equal(booking.BookingId, payment.Result.BookingId);
                Assert.Equal(booking.TotalPrice, payment.Result.Amount);
            }
        }

        [Fact]
        public async void ProcessPayment()
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var booking = new Booking();
                booking.TotalPrice = 100;
                var paymentService = new PaymentService(context);
                var payment = paymentService.CreateFromBooking(booking);

                //Act

                await paymentService.StartPayment(payment.Result);

                //Assert

                Assert.Equal("OK", payment.Result.IncomingPaymentStatus);
            }
        }
    }
}
