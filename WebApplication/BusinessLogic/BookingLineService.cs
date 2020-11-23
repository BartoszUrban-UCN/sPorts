using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic.Shared;

namespace WebApplication.BusinessLogic
{
    public class BookingLineService : ServiceBase, IBookingLineService
    {
        public BookingLineService(SportsContext context) : base(context)
        { }

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

        public async Task AddTime(int? id, int amount)
        {
            var bookingLine = await GetSingle(id);

            bookingLine.EndDate = bookingLine.EndDate.AddSeconds(amount);

            _context.Update(bookingLine);
        }

        public async Task CancelBookingLine(int? id)
        {
            var bookingLine = await GetSingle(id);

            if (bookingLine.Ongoing)
            {
                bookingLine.Ongoing = false;
                _context.BookingLines.Update(bookingLine);
            }
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();
            return await _context.BookingLines.AnyAsync(b => b.BookingLineId == id);
        }
    }
}
