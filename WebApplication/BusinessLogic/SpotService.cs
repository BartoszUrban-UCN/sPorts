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

        public async Task<int> CreateWithLocation(Spot spot, Location location)
        {
            var locationId = _locationService.Create(location);
            spot.LocationId = await locationId;

            return await Create(spot);
        }

        public Task Delete(int? idOfTheObjectToDelete)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteSpotLocation(Spot spot)
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

        public Task<Spot> UpdateSpotLocation(Spot spot)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Exists(int? id)
        {
            throw new System.NotImplementedException();
        }
    }
}
