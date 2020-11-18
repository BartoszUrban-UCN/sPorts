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
            _context.Add(spot);
            await _context.SaveChangesAsync();
            return spot.SpotId;
        }

        public Task Delete(int? idOfTheObjectToDelete)
        {
            throw new System.NotImplementedException();
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
                .FirstOrDefaultAsync(m => m.SpotId == id);

            return await spot;
        }

        public Task<Spot> Update(Spot objectToUpdate)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Exists(int? id)
        {
            throw new System.NotImplementedException();
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
            await _locationService.Delete(spot.LocationId);
        }
    }
}
