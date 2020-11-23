using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class SpotService : ISpotService
    {
        private readonly SportsContext _context;
        private readonly ILocationService _locationService;
        public SpotService(SportsContext context, ILocationService locationService)
        {
            // if (context == null)
            //     throw new BusinessException("SpotService", "The context argument was null.");

            // if (locationService == null)
            //     throw new BusinessException("SpotService", "The locationService argument was null.");

            _context = context;
            _locationService = locationService;
        }

        public async Task<int> Create(Spot spot)
        {
            // TODO Might not be necesary
            // TODO Remove if the caller already checks for null
            if (spot == null)
            {
                throw new BusinessException("Create", "Spot object is null.");
            }

            _context.Spots.Add(spot);
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Create", "The Spot was not created.");

                return spot.SpotId;
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

        public async Task Delete(int? id)
        {
            var spot = await GetSingle(id);
            _context.Spots.Remove(spot);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Delete", "Couldn't delete the Spot.");
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

        public async Task<IEnumerable<Spot>> GetAll()
        {
            var spots = await _context.Spots
                .Include(spot => spot.Marina)
                .Include(spot => spot.Location)
                .ToListAsync();

            return spots;
        }

        public async Task<Spot> GetSingle(int? id)
        {
            if (id == null)
                throw new BusinessException("GetSingle", "Id is null.");

            if (id < 0)
                throw new BusinessException("GetSingle", "The id was negative.");

            var spot = await _context.Spots
                .Include(s => s.Marina)
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.SpotId == id);

            if (spot == null)
                throw new BusinessException("GetSingle", $"Didn't find Spot with id {id}");

            return spot;
        }

        public async Task<Spot> Update(Spot spot)
        {
            _context.Spots.Update(spot);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Update", "The Spot was not updated.");

                return spot;
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

        public Task<bool> Exists(int? id)
        {
            if (id < 0)
                throw new BusinessException("Exists", "The id is negative.");

            return _context.Spots.AnyAsync(l => l.SpotId == id);
        }

        public async Task<int> CreateWithLocation(Spot spot, Location location)
        {
            var locationId = _locationService.Create(location);
            spot.LocationId = await locationId;

            return await Create(spot);
        }

        public async Task<Spot> UpdateSpotLocation(Spot spot, Location location)
        {
            if (spot.LocationId == null)
            {
                spot.LocationId = await _locationService.Create(location);
            }
            else
            {
                location.LocationId = (int)spot.LocationId;
                _context.Update(location);
            }

            return spot;
        }

        public async Task<Spot> DeleteSpotLocation(Spot spot)
        {
            spot = await GetSingle(spot.SpotId);
            await _locationService.Delete(spot.LocationId);
            return spot;
        }
    }
}
