using System.Collections.Generic;
using System.Linq;
using WebApplication.Data;
using WebApplication.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.BusinessLogic
{
    public class BoatOwnerService : IBoatOwnerService
    {
        private readonly SportsContext _context;

        public BoatOwnerService(SportsContext context)
        {
            _context = context;
        }

        public async Task<BoatOwner> FindBoatOwner(int boatOwnerId)
        {
            var boatOwnersWithBoatsAndBookings = await _context.BoatOwners.Include(bo => bo.Boats)
                                            .ThenInclude(b => b.Bookings)
                                            .ThenInclude(b => b.BookingLines)
                                            .ToListAsync();
            var boatOwner = boatOwnersWithBoatsAndBookings.FirstOrDefault(b => b.BoatOwnerId == boatOwnerId);

            if (boatOwner == null)
            {
                throw new BusinessException("boatownersevice", $"Didn't find Boat Owner with id {boatOwnerId}");
            }

            return boatOwner;
        }
        public async Task<IEnumerable<Booking>> Bookings(int boatOwnerId)
        {
            var boatOwner = await FindBoatOwner(boatOwnerId);

            var bookings = from boat in boatOwner.Boats
                       from booking in boat.Bookings
                       select booking;

            return bookings;
        }

        public async Task<IEnumerable<Booking>> OngoingBookings(int boatOwnerId)
        {
            var boatOwner = await FindBoatOwner(boatOwnerId);

            var ongoingBookings = OngoingBookings(boatOwner);

            return ongoingBookings;
        }

        public IEnumerable<Booking> OngoingBookings(BoatOwner boatOwner)
        {
            if (boatOwner == null)
            {
                throw new BusinessException("boatownerservice", "The parameter can't be null.");
            }

            var boats = boatOwner.Boats;
            var bookingsToReturn = from boat in boats
                                   from booking in boat.Bookings
                                   where HasOngoing(booking)
                                   select booking;

            return bookingsToReturn;
        }

        public bool HasOngoing(Booking booking)
        {
            if (booking == null)
            {
                throw new BusinessException("boatownerservice", "The parameter can't be null.");
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
                throw new BusinessException("boatownerservice", "The parameter can't be null.");
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
                throw new BusinessException("boatownerservice", "The parameter can't be null.");
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
                throw new BusinessException("boatownerservice", "The parameter can't be null.");
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
                throw new BusinessException("boatownerservice", "The parameter can't be null.");
            }

            if (bookingLine.StartDate == null || bookingLine.EndDate == null)
            {
                throw new BusinessException("boatownerservice", "Either the startdate or the enddate is null.");
            }

            return bookingLine.EndDate - bookingLine.StartDate;
        }
    }
}
