using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BookingLineService : ServiceBase, IBookingLineService
    {
        public BookingLineService(SportsContext context) : base(context)
        {
        }

        public async Task<int> Create(BookingLine bookingLine)
        {
            // TODO Might not be necesary
            // TODO Remove if the caller already checks for null
            if (bookingLine == null)
            {
                throw new BusinessException("Create", "Booking Line object is null.");
            }

            _context.BookingLines.Add(bookingLine);
            return bookingLine.BookingLineId;
        }

        public async Task<BookingLine> GetSingle(int? id)
        {
            if (id == null)
                throw new BusinessException("GetSingle", "Id is null.");

            if (id < 0)
                throw new BusinessException("GetSingle", "The id is negative.");

            var bookingLine = await _context.BookingLines
                                             .Include(b => b.Spot)
                                                .ThenInclude(s => s.Marina)
                                                    .ThenInclude(m => m.MarinaOwner)
                                                        .ThenInclude(m => m.Person)
                                            .FirstOrDefaultAsync(b => b.BookingLineId == id);

            if (bookingLine == null)
                throw new BusinessException("GetSingle", $"Didn't find Booking Line with id {id}");

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

        public async Task<BookingLine> Update(BookingLine bookingLine)
        {
            _context.BookingLines.Update(bookingLine);
            return bookingLine;
        }

        public async Task Delete(int? id)
        {
            var bookingLine = await GetSingle(id);
            _context.BookingLines.Remove(bookingLine);
        }

        public async Task<bool> AddTime(int? id, int amount)
        {
            try
            {
                if (amount > 0)
                {
                    var bookingLine = await GetSingle(id);

                    bookingLine.EndDate = bookingLine.EndDate.AddMinutes(amount);
                    _context.BookingLines.Update(bookingLine);

                    var result = _context.SaveChanges();
                    if (result < 1)
                        throw new BusinessException("AddTime", "Did not add time.");

                    return true;
                }

                return false;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException("AddTime", "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException("AddTime", "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
        }

        public async Task<bool> CancelBookingLine(int? id)
        {
            try
            {
                var bookingLine = await GetSingle(id);

                if (bookingLine.Ongoing)
                {
                    bookingLine.Ongoing = false;
                    _context.BookingLines.Update(bookingLine);

                    var result = _context.SaveChanges();
                    if (result < 1)
                        throw new BusinessException("CancelBookingLine", "The Booking Line was not cancelled.");

                    return true;
                }

                return false;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException("CancelBookingLine", "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException("CancelBookingLine", "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
        }

        public async Task<bool> Exists(int? id)
        {
            if (id < 0)
                throw new BusinessException("Exists", "The id is negative.");

            return await _context.BookingLines.AnyAsync(b => b.BookingLineId == id);
        }
    }
}
