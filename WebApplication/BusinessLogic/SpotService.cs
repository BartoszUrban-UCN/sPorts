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
            _context = context;
            _locationService = locationService;
        }

        public async Task<int> Create(Spot spot)
        {
            _context.Spots.Add(spot);
            await _context.SaveChangesAsync();
            return spot.SpotId;
        }

        public async Task Delete(int? spotId)
        {
            var location = await _context.Spots.FindAsync(spotId);
            _context.Spots.Remove(location);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Spot>> GetAll()
        {
            var sportsContext = _context.Spots
                .Include(spot => spot.Marina)
                .Include(spot => spot.Location);

            return await sportsContext.ToListAsync();
        }

        public async Task<Spot> GetSingle(int? id)
        {
            var spot = _context.Spots
                .Include(s => s.Marina)
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.SpotId == id);

            return await spot;
        }

        public async Task<Spot> Update(Spot spot)
        {
            _context.Update(spot);
            await _context.SaveChangesAsync();
            return spot;
        }

        public Task<bool> Exists(int? id)
        {
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
                _context.Update(location);
            }

            //await _context.SaveChangesAsync();

            return spot;
        }

        public async Task DeleteSpotLocation(Spot spot)
        {
            var locationId = spot.LocationId;

            spot.LocationId = null;

            await Update(spot);

            await _locationService.Delete(locationId);
        }
    }
}
