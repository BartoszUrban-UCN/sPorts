using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BoatOwnerService : ServiceBase, IBoatOwnerService
    {
        private readonly IBookingService _bookingService;

        public BoatOwnerService(SportsContext context, IBookingService bookingService) : base(context)
        {
            _bookingService = bookingService;
        }

        public async Task<int> Create(BoatOwner boatOwner)
        {
            // TODO Might not be necesary
            // TODO Remove if the caller already checks for null
            if (boatOwner == null)
            {
                throw new BusinessException("Create", "Boat Owner object is null.");
            }

            _context.BoatOwners.Add(boatOwner);
            return boatOwner.BoatOwnerId;
        }

        public async Task<BoatOwner> GetSingle(int? id)
        {
            if (id == null)
                throw new BusinessException("GetSingle", "Id is null.");

            if (id < 0)
                throw new BusinessException("GetSingle", "Id is negative.");

            var boatOwner = await _context.BoatOwners
                                            .Include(b => b.Person)
                                            .Include(b => b.Boats)
                                                .ThenInclude(b => b.Bookings)
                                                    .ThenInclude(b => b.BookingLines)
                                            .FirstOrDefaultAsync(b => b.BoatOwnerId == id);

            if (boatOwner == null)
                throw new BusinessException("GetSingle", $"Didn't find Boat Owner with id {id}");

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

        public async Task<BoatOwner> Update(BoatOwner boatOwner)
        {
            _context.BoatOwners.Update(boatOwner);
            return boatOwner;
        }

        public async Task Delete(int? id)
        {
            var boatOwner = await GetSingle(id);
            _context.BoatOwners.Remove(boatOwner);
        }

        public async Task<bool> Exists(int? id)
        {
            if (id < 0)
                throw new BusinessException("Exists", "The id is negative.");

            return await _context.BoatOwners.AnyAsync(b => b.BoatOwnerId == id);
        }

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
            if (boatOwner == null)
                throw new BusinessException("GetOngoingBookings", "The boatOwner argument was null.");

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
            if (booking == null)
            {
                throw new BusinessException("HasOngoing", "The parameter can't be null.");
            }

            foreach (var bookingLine in booking.BookingLines)
            {
                if (bookingLine.Ongoing)
                    return true;
            }
            return false;
        }

        public double MoneySpent(BoatOwner boatOwner)
        {
            if (boatOwner == null)
            {
                throw new BusinessException("MoneySpent", "The parameter can't be null.");
            }

            var boats = boatOwner.Boats;
            var moneySpent = (from boat in boats
                              from booking in boat.Bookings
                              select booking.TotalPrice).Sum();

            return moneySpent;
        }

        public TimeSpan TotalTime(BoatOwner boatOwner)
        {
            if (boatOwner == null)
            {
                throw new BusinessException("TotalTime", "The parameter can't be null.");
            }

            TimeSpan totalTime = new TimeSpan();

            var boats = boatOwner.Boats;
            var bookings = (from boat in boats
                            from booking in boat.Bookings
                            select booking).ToList();

            foreach (var booking in bookings)
            {
                totalTime += TotalTime(booking);
            }

            return totalTime;
        }

        public TimeSpan TotalTime(Booking booking)
        {
            if (booking == null)
            {
                throw new BusinessException("TotalTime", "The parameter can't be null.");
            }

            TimeSpan totalTime = new TimeSpan();

            foreach (var bookingLine in booking.BookingLines)
            {
                totalTime += TotalTime(bookingLine);
            }

            return totalTime;
        }

        public TimeSpan TotalTime(BookingLine bookingLine)
        {
            if (bookingLine == null)
            {
                throw new BusinessException("TotalTime", "The parameter can't be null.");
            }

            return bookingLine.EndDate - bookingLine.StartDate;
        }
    }
}
