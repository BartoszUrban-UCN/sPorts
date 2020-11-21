using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using WebApplication.Data;
using WebApplication.Models;

using Microsoft.EntityFrameworkCore;

namespace WebApplication.BusinessLogic
{
    public class BoatOwnerService : IBoatOwnerService
    {
        private readonly SportsContext _context;
        private readonly IBookingService _bookingService;

        public BoatOwnerService(SportsContext context, IBookingService bookingService)
        {
            // if (context == null)
            //     throw new BusinessException("BoatOwnerService", "The context argument was null.");
            
            // if (bookingService == null)
            //     throw new BusinessException("BoatOwnerService", "The bookingService argument was null.");

            _context = context;
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
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Create", "The Booking Line was not created.");

                return boatOwner.BoatOwnerId;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException("Create", "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException("Create", "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
        }

        public async Task<BoatOwner> GetSingle(int? id)
        {
            if (id == null)
            {
                throw new BusinessException("GetSingle", "Id is null.");
            }

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

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Update", "The Booking Line was not updated.");

                return boatOwner;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException("Update", "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException("Update", "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
        }

        public async Task Delete(int? id)
        {
            var boatOwner = await GetSingle(id);
            _context.BoatOwners.Remove(boatOwner);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Delete", "Couldn't delete the Booking Line.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException("Delete", "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException("Delete", "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
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
