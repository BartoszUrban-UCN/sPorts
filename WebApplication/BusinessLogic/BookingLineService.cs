using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingLineService : IBookingLineService
    {
        private readonly SportsContext _context;

        public BookingLineService(SportsContext context)
        {
            _context = context;
        }

        public async Task<BookingLine> FindBookingLine(int id)
        {
            var bookingLine = await _context.BookingLines
                                            .FirstOrDefaultAsync(b => b.BookingLineId == id);
            if (bookingLine == null)
            {
                throw new BusinessException("bookinglineservice", $"The bookingline with id {id} was not found.");
            }

            return bookingLine;
        }

        public async Task<bool> AddTime(int id, int amount)
        {
            var success = false;

            try
            {
                if (amount > 0)
                {
                    var bookingLine = await FindBookingLine(id);

                    bookingLine.EndDate = bookingLine.EndDate.AddMinutes(amount);
                    _context.BookingLines.Update(bookingLine);
                    var result = _context.SaveChanges();
                    success = result > 0;
                }
            }
            catch (Exception ex) when (ex is DbUpdateException
                                    || ex is DbUpdateConcurrencyException
                                    || ex is BusinessException)
            {
                throw new BusinessException("Booking line update", "Failed to add more time to your booking line.");
            }
            return success;
        }

        public async Task<bool> CancelBookingLine(int id)
        {
            var success = false;
            try
            {
                var bookingLine = await FindBookingLine(id);

                if (bookingLine.Ongoing)
                {
                    bookingLine.Ongoing = false;
                    _context.BookingLines.Update(bookingLine);
                    var result = _context.SaveChanges();
                    success = result > 0;
                }
            }
            catch (Exception ex) when (ex is DbUpdateException
                                    || ex is DbUpdateConcurrencyException
                                    || ex is BusinessException)
            {
                throw new BusinessException("Booking line cancellation", "Failed to cancel your booking line.");
            }
            return success;
        }

        private BookingLine ExplicitLoad(BookingLine bookingLine)
        {
            _context.Entry(bookingLine).Reference(bl => bl.Spot).Load();
            _context.Entry(bookingLine.Spot).Reference(s => s.Location).Load();
            _context.Entry(bookingLine.Spot).Reference(s => s.Marina).Load();
            _context.Entry(bookingLine.Spot.Marina).Reference(m => m.Address).Load();
            _context.Entry(bookingLine.Spot.Marina).Reference(m => m.MarinaOwner).Load();
            _context.Entry(bookingLine.Spot.Marina.MarinaOwner).Reference(mo => mo.Person).Load();
            _context.Entry(bookingLine).Reference(bl => bl.Booking).Load();
            _context.Entry(bookingLine.Booking).Reference(b => b.Boat).Load();
            _context.Entry(bookingLine.Booking.Boat).Reference(b => b.BoatOwner).Load();
            _context.Entry(bookingLine.Booking.Boat.BoatOwner).Reference(bo => bo.Person).Load();
            return bookingLine;
        }

        public async Task<List<BookingLine>> GetBookingLinesByMarinaOwner(int marinaOwnerId)
        {
            List<BookingLine> marinaOwnerBookings = new List<BookingLine>(await _context.BookingLines.ToListAsync());
            marinaOwnerBookings.ForEach(bl => ExplicitLoad(bl));

            return marinaOwnerBookings.FindAll(bl => bl.Spot.Marina.MarinaOwner.MarinaOwnerId == marinaOwnerId);
        }
    }
}