using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BoatOwnerService : ServiceBase, IBoatOwnerService
    {
        private readonly IBookingService _bookingService;

        public BoatOwnerService(SportsContext context, IBookingService bookingService) : base(context)
        {
            bookingService.ThrowIfNull();
            _bookingService = bookingService;
        }

        public async Task<int> Create(BoatOwner boatOwner)
        {
            boatOwner.ThrowIfNull();

            await _context.BoatOwners.AddAsync(boatOwner);

            return boatOwner.BoatOwnerId;
        }

        public async Task<BoatOwner> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var boatOwner = await _context.BoatOwners
                                            .Include(b => b.Person)
                                            .Include(b => b.Boats)
                                                .ThenInclude(b => b.Bookings)
                                                    .ThenInclude(b => b.BookingLines)
                                            .FirstOrDefaultAsync(b => b.BoatOwnerId == id);

            boatOwner.ThrowIfNull();

            return boatOwner;
        }

        public async Task<IEnumerable<BoatOwner>> GetAll()
        {
            var boatsOwners = await _context.BoatOwners
                                            .Include(b => b.Person)
                                            .Include(b => b.Boats)
                                                .ThenInclude(b => b.Bookings)
                                                    .ThenInclude(b => b.BookingLines)
                                            .ToListAsync();
            return boatsOwners;
        }

        public BoatOwner Update(BoatOwner boatOwner)
        {
            _context.Update(boatOwner);

            return boatOwner;
        }

        public async Task Delete(int? id)
        {
            var boatOwner = await GetSingle(id);

            _context.Remove(boatOwner);
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();

            return await _context.BoatOwners.AnyAsync(b => b.BoatOwnerId == id);
        }

        // get bookinsg
        public async Task<IEnumerable<Booking>> GetBookings(int? id)
        {
            var boatOwner = await GetSingle(id);

            var bookings = from boat in boatOwner.Boats
                           from booking in boat.Bookings
                           select booking;

            return bookings;
        }

        public async Task<IEnumerable<Booking>> GetOngoingBookings(int? id)
        {
            var boatOwner = await GetSingle(id);

            var ongoingBookings = GetOngoingBookings(boatOwner);

            return ongoingBookings;
        }

        public IEnumerable<Booking> GetOngoingBookings(BoatOwner boatOwner)
        {
            boatOwner.ThrowIfNull();

            var boats = boatOwner.Boats;
            var bookingsToReturn = from boat in boats
                                   from booking in boat.Bookings
                                   where HasOngoing(booking)
                                   select booking;

            return bookingsToReturn;
        }

        public async Task<IEnumerable<BookingLine>> GetOngoingBookingLines(int? bookingId)
        {
            return await _bookingService.GetOngoingBookingLines(bookingId);
        }

        public async Task<IEnumerable<BookingLine>> GetBookingLines(int? bookingId)
        {
            return await _bookingService.GetBookingLines(bookingId);
        }

        public async Task<IEnumerable<Boat>> GetBoats(int? id)
        {
            var boatOwner = await GetSingle(id);
            var boats = boatOwner.Boats;

            return boats;
        }
        public bool HasOngoing(Booking booking)
        {
            booking.ThrowIfNull();

            foreach (var bookingLine in booking.BookingLines)
                if (bookingLine.Ongoing)
                    return true;

            return false;
        }

        public double MoneySpent(BoatOwner boatOwner)
        {
            boatOwner.ThrowIfNull();

            var boats = boatOwner.Boats;
            var moneySpent = (from boat in boats
                              from booking in boat.Bookings
                              select booking.TotalPrice).Sum();

            return moneySpent;
        }

        public TimeSpan TotalTime(BoatOwner boatOwner)
        {
            boatOwner.ThrowIfNull();

            TimeSpan totalTime = new TimeSpan();

            var boats = boatOwner.Boats;
            var bookings = (from boat in boats
                            from booking in boat.Bookings
                            select booking).ToList();

            foreach (var booking in bookings)
                totalTime += TotalTime(booking);

            return totalTime;
        }

        public TimeSpan TotalTime(Booking booking)
        {
            booking.ThrowIfNull();

            TimeSpan totalTime = new TimeSpan();

            foreach (var bookingLine in booking.BookingLines)
                totalTime += TotalTime(bookingLine);

            return totalTime;
        }

        public TimeSpan TotalTime(BookingLine bookingLine)
        {
            bookingLine.ThrowIfNull();

            return bookingLine.EndDate - bookingLine.StartDate;
        }
    }
}
