using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingLineService : ServiceBase, IBookingLineService
    {

        private readonly IBookingFormService _bookingFormService;

        public BookingLineService(SportsContext context, IBookingFormService bookingFormService) : base(context)
        {
            _bookingFormService = bookingFormService;
        }

        public async Task<int> Create(BookingLine bookingLine)
        {
            bookingLine.ThrowIfNull();
            await _context.AddAsync(bookingLine);

            return bookingLine.BookingLineId;
        }

        public async Task<BookingLine> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var bookingLine = await _context.BookingLines
                                             .Include(b => b.Spot)
                                                .ThenInclude(s => s.Marina)
                                                    .ThenInclude(m => m.MarinaOwner)
                                                        .ThenInclude(m => m.Person)
                                              .Include(b => b.Booking)
                                                  .ThenInclude(bk => bk.Boat)
                                                      .ThenInclude(bt => bt.BoatOwner)
                                                          .ThenInclude(bo => bo.Person)
                                            .FirstOrDefaultAsync(b => b.BookingLineId == id);

            bookingLine.ThrowIfNull();

            return bookingLine;
        }

        public async Task<IEnumerable<BookingLine>> GetAll()
        {
            var bookingLines = await _context.BookingLines
                                             .Include(b => b.Spot)
                                                .ThenInclude(s => s.Marina)
                                                    .ThenInclude(m => m.MarinaOwner)
                                                        .ThenInclude(m => m.Person)
                                                .Include(b => b.Booking)
                                                    .ThenInclude(bk => bk.Boat)
                                                        .ThenInclude(bt => bt.BoatOwner)
                                                            .ThenInclude(bo => bo.Person)
                                             .ToListAsync();
            return bookingLines;
        }

        public BookingLine Update(BookingLine bookingLine)
        {
            bookingLine.ThrowIfNull();

            _context.BookingLines.Update(bookingLine);

            return bookingLine;
        }

        public async Task Delete(int? id)
        {
            var bookingLine = await GetSingle(id);
            _context.BookingLines.Remove(bookingLine);
        }

        public async Task<BookingLine> AddTime(int? id, int amount)
        {
            var bookingLine = await GetSingle(id);
            var newEndDate = bookingLine.EndDate.AddDays(amount);

            List<Spot> availableSpots = new List<Spot>(await _bookingFormService.GetAvailableSpots(bookingLine.Spot.Marina.MarinaId, bookingLine.Booking.BoatId, bookingLine.StartDate, newEndDate));

            if (bookingLine.StartDate > DateTime.Now && bookingLine.EndDate < DateTime.Now && availableSpots.Contains(bookingLine.Spot))
            {
                bookingLine.EndDate = bookingLine.EndDate.AddDays(amount);
                return bookingLine;
            }

            return null;
        }

        public async Task CancelBookingLine(int? id)
        {
            var bookingLine = await GetSingle(id);

            if (bookingLine.Ongoing)
            {
                bookingLine.Ongoing = false;
                _context.BookingLines.Update(bookingLine);
                await Save();
            }
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();
            return await _context.BookingLines.AnyAsync(b => b.BookingLineId == id);
        }
    }
}
