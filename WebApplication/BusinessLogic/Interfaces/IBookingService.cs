using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingService : ICRUD<Booking>
    {
        Booking CreateBookingLine(Booking booking, DateTime startDate, DateTime endDate, Spot spot);

        Task<Booking> LoadSpots(Booking booking);

        Dictionary<Marina, IEnumerable<BookingLine>> FilterLinesByMarina(Booking booking);

        Task<Booking> SaveBooking(Booking booking);

        Task<BookingLine> GetBookingLine(int? id);

        Task<IEnumerable<BookingLine>> GetBookingLines(int? id);

        Task<IEnumerable<BookingLine>> GetOngoingBookingLines(int? id);

        Task<bool> ConfirmSpotBooked(int bookingLineId);

        Task CancelBooking(int? id);

        Task<Booking> ValidateShoppingCart(Booking booking);

        Task<IEnumerable<BookingLine>> InvalidBookingLines(Booking booking);

        Booking CartRemoveBookingLine(Booking booking, DateTime startDate);

        Task<List<KeyValuePair<Marina, int>>> GetAllAvailableSpotsCount(IList<int> marinaIds, int boatId, DateTime startDate, DateTime endDate);

        double CalculateTotalPrice(Booking booking);
        double CalculateTotalDiscount(Booking booking);
    }
}
