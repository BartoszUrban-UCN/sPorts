﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public interface IBookingService : ICRUD<Booking>
    {
        List<BookingLine> CreateBookingLines(Dictionary<DateTime[], Spot> marinaSpotStayDates);

        Task<BookingLine> GetBookingLine(int? id);

        Task<IEnumerable<BookingLine>> GetBookingLines(int? id);

        Task<IEnumerable<BookingLine>> GetOngoingBookingLines(int? id);

        Task<bool> ConfirmSpotBooked(int bookingLineId);

        Task CancelBooking(int? id);

        Task<Booking> ValidateShoppingCart(Booking booking);

        Booking CartRemoveBookingLine(Booking booking, BookingLine bookingLine);

        Task<Dictionary<int, int>> GetAllAvailableSpotsCount(IList<int> marinaIds, string boatName, DateTime startDate, DateTime endDate);
    }
}
