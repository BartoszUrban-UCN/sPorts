using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class SpotService : ServiceBase, ISpotService
    {
        private readonly ILocationService _locationService;
        public SpotService(SportsContext context, ILocationService locationService) : base(context)
        {
            // if (context == null)
            //     throw new BusinessException("SpotService", "The context argument was null.");

            // if (locationService == null)
            //     throw new BusinessException("SpotService", "The locationService argument was null.");
            _locationService = locationService;
        }

        public async Task<int> Create(Spot spot)
        {
            if (spot is not null)
                await _context.Spots.AddAsync(spot);

            return spot.SpotId;
        }

        public async Task Delete(int? id)
        {
            var spot = await GetSingle(id);
            _context.Spots.Remove(spot);
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
            return spot;
        }

        public Task<bool> Exists(int? id)
        {
            if (id < 0)
                throw new BusinessException("Exists", "The id is negative.");

            return _context.Spots.AnyAsync(l => l.SpotId == id);
        }

        public async Task<Spot> CreateWithLocation(Spot spot, Location location)
        {
            var locationId = await _locationService.Create(location);
            spot.LocationId = locationId.LocationId;

            return await Create(spot);
        }

        public async Task<Spot> UpdateSpotLocation(Spot spot, Location location)
        {
            if (spot.LocationId == null)
            {
                spot.LocationId = (await _locationService.Create(location)).LocationId;
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
