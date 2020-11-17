using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    [ApiExplorerSettings(IgnoreApi = true)]
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

                    bookingLine.EndDate = bookingLine.EndDate.AddSeconds(amount);

                    var result = _context.SaveChanges();
                    success = result > 0;
                }
            }
            catch (Exception ex) when (ex is DbUpdateException
                                    || ex is DbUpdateConcurrencyException
                                    || ex is BusinessException)
            { }
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

                    var result = _context.SaveChanges();
                    success = result > 0;
                }
            }
            catch (Exception ex) when (ex is DbUpdateException
                                    || ex is DbUpdateConcurrencyException
                                    || ex is BusinessException)
            { }
            return success;
        }
    }
}