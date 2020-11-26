using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic.Shared;

namespace WebApplication.BusinessLogic
{
    public class MarinaOwnerService : ServiceBase, IMarinaOwnerService
    {
        private readonly IBookingLineService _bookingLineService;

        public MarinaOwnerService(SportsContext context, IBookingLineService bookingLineService) : base(context)
        {
            _bookingLineService = bookingLineService;
        }

        public async Task<int> Create(MarinaOwner marinaOwner)
        {
            marinaOwner.ThrowIfNull();

            await _context.AddAsync(marinaOwner);

            return marinaOwner.MarinaOwnerId;
        }

        public async Task<MarinaOwner> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var marinaOwner = await _context.MarinaOwners
                                            .Include(b => b.Person)
                                            .Include(m => m.Marinas).ThenInclude(m => m.Spots).ThenInclude(s => s.BookingLines)
                                            .Include(m => m.Marinas).ThenInclude(m => m.Spots).ThenInclude(s => s.Location)
                                            .FirstOrDefaultAsync(b => b.MarinaOwnerId == id);

            marinaOwner.ThrowIfNull();

            return marinaOwner;
        }

        public async Task<IEnumerable<MarinaOwner>> GetAll()
        {
            var marinaOwners = await _context.MarinaOwners
                                            .Include(b => b.Person)
                                            .Include(m => m.Marinas).ThenInclude(m => m.Spots).ThenInclude(s => s.BookingLines).ThenInclude(b => b.Booking)
                                            .Include(m => m.Marinas).ThenInclude(m => m.Spots).ThenInclude(s => s.Location)
                                            .ToListAsync();
            return marinaOwners;
        }

        public MarinaOwner Update(MarinaOwner marinaOwner)
        {
            marinaOwner.ThrowIfNull();
            _context.MarinaOwners.Update(marinaOwner);
            return marinaOwner;
        }

        public async Task Delete(int? id)
        {
            id.ThrowIfInvalidId();
            var marinaOwner = await GetSingle(id);
            _context.MarinaOwners.Remove(marinaOwner);
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();
            return await _context.MarinaOwners.AnyAsync(b => b.MarinaOwnerId == id);
        }

        public async Task<IEnumerable<BookingLine>> GetBookingLines(int marinaOwnerId)
        {
            marinaOwnerId.ThrowIfNegativeId();

            var bookingLines = (List<BookingLine>)await _bookingLineService.GetAll();

            return bookingLines.FindAll(b => b.Spot.Marina.MarinaOwner.MarinaOwnerId == marinaOwnerId);
        }

        public async Task<IEnumerable<BookingLine>> GetUnconfirmedBookingLines(int marinaOwnerId)
        {
            marinaOwnerId.ThrowIfNegativeId();
            var bookingLines = (List<BookingLine>)await GetBookingLines(marinaOwnerId);

            return bookingLines.FindAll(b => !b.Confirmed);
        }
        public async Task<IEnumerable<BookingLine>> GetConfirmedBookingLines(int marinaOwnerId)
        {
            marinaOwnerId.ThrowIfNegativeId();
            var bookingLines = (List<BookingLine>)await GetBookingLines(marinaOwnerId);

            return bookingLines.FindAll(b => b.Confirmed);
        }
    }
}
