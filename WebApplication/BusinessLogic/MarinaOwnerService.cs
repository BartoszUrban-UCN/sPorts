using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class MarinaOwnerService : IMarinaOwnerService
    {
        private readonly SportsContext _context;
        private readonly IBookingService _bookingLinesService;

        public MarinaOwnerService(SportsContext context, IBookingService bookingLinesService)
        {
            // if (context == null)
            //     throw new BusinessException("MarinaOwnerService", "The context argument was null.");

            // if (bookingLinesService == null)
            //     throw new BusinessException("MarinaOwnerService", "The bookingLinesService argument was null.");

            _context = context;
            _bookingLinesService = bookingLinesService;
        }

        public async Task<int> Create(MarinaOwner marinaOwner)
        {
            // TODO Might not be necesary
            // TODO Remove if the caller already checks for null
            if (marinaOwner == null)
            {
                throw new BusinessException("Create", "Marina Owner object is null.");
            }

            _context.MarinaOwners.Add(marinaOwner);
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Create", "The Marina Owner was not created.");

                return marinaOwner.MarinaOwnerId;
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

        public async Task<MarinaOwner> GetSingle(int? id)
        {
            if (id == null)
                throw new BusinessException("GetSingle", "Id is null.");

            if (id < 0)
                throw new BusinessException("GetSingle", "id is negative.");

            var marinaOwner = await _context.MarinaOwners
                                            .Include(b => b.Person)
                                            .Include(m => m.Marina).ThenInclude(m => m.Spots).ThenInclude(s => s.BookingLines)
                                            .Include(m => m.Marina).ThenInclude(m => m.Spots).ThenInclude(s => s.Location)
                                            .FirstOrDefaultAsync(b => b.MarinaOwnerId == id);

            if (marinaOwner == null)
                throw new BusinessException("GetSingle", $"Didn't find Marina Owner with id {id}");

            return marinaOwner;
        }

        public async Task<IEnumerable<MarinaOwner>> GetAll()
        {
            var marinaOwners = await _context.MarinaOwners
                                            .Include(b => b.Person)
                                            .Include(m => m.Marina).ThenInclude(m => m.Spots).ThenInclude(s => s.BookingLines).ThenInclude(b => b.Booking)
                                            .Include(m => m.Marina).ThenInclude(m => m.Spots).ThenInclude(s => s.Location)
                                            .ToListAsync();
            return marinaOwners;
        }

        public async Task<MarinaOwner> Update(MarinaOwner marinaOwner)
        {
            _context.MarinaOwners.Update(marinaOwner);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Update", "The Marina Owner was not updated.");

                return marinaOwner;
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
            var marinaOwner = await GetSingle(id);
            _context.MarinaOwners.Remove(marinaOwner);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Delete", "Couldn't delete the Marina Owner.");
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

            return await _context.MarinaOwners.AnyAsync(b => b.MarinaOwnerId == id);
        }

        public async Task<IEnumerable<BookingLine>> GetBookingLines(int marinaOwnerId)
        {
            if (marinaOwnerId < 0)
                throw new BusinessException("GetBookingLines", "The id is negative.");

            var bookingLines = (List<BookingLine>)await _bookingLinesService.GetAll();

            return bookingLines.FindAll(b => b.Spot.Marina.MarinaOwner.MarinaOwnerId == marinaOwnerId);
        }

        public async Task<IEnumerable<BookingLine>> GetUnconfirmedBookingLines(int marinaOwnerId)
        {
            var bookingLines = (List<BookingLine>)await GetBookingLines(marinaOwnerId);

            return bookingLines.FindAll(b => !b.Confirmed);
        }
        public async Task<IEnumerable<BookingLine>> GetConfirmedBookingLines(int marinaOwnerId)
        {
            var bookingLines = (List<BookingLine>)await GetBookingLines(marinaOwnerId);

            return bookingLines.FindAll(b => b.Confirmed);
        }
    }
}
