using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic.Shared;

namespace WebApplication.BusinessLogic
{
    public class SpotService : ServiceBase, ISpotService
    {
        private readonly ILocationService _locationService;
        public SpotService(SportsContext context, ILocationService locationService) : base(context)
        {
            _locationService = locationService;
        }

        public async Task<int> Create(Spot spot)
        {
            spot.ThrowIfNull();
            await _context.AddAsync(spot);
            return spot.SpotId;
        }

        public async Task Delete(int? id)
        {
            var spot = await GetSingle(id);
            _context.Remove(spot);
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
            id.ThrowIfInvalidId();

            var spot = await _context.Spots
                .Include(s => s.Marina)
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.SpotId == id);

            spot.ThrowIfNull();

            return spot;
        }

        public Spot Update(Spot spot)
        {
            _context.Update(spot);
            return spot;
        }

        public Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();
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
