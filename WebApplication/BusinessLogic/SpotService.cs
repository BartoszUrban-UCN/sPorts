using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class SpotService : ServiceBase, ISpotService
    {
        private readonly ILocationService _locationService;
        private readonly UserService _userService;

        public SpotService(SportsContext context, ILocationService locationService, UserService userService) : base(context)
        {
            _locationService = locationService;
            _userService = userService;
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

        private IIncludableQueryable<Spot, Location> LoadSpots()
        {
            var spots = _context.Spots
                .Include(spot => spot.Marina)
                    .ThenInclude(marina => marina.MarinaOwner)
                        .ThenInclude(marinaOwner => marinaOwner.Person)
                .Include(spot => spot.Location);
            
            return spots;
        }
        
        public async Task<IEnumerable<Spot>> GetAll()
        {
            var spots = await LoadSpots().ToListAsync();

            return spots;
        }

        public async Task<IEnumerable<Spot>> GetAll(int marinaOwnerId)
        {
            marinaOwnerId.ThrowIfNegativeId();

            var spots = await LoadSpots()
                .Where(predicate => predicate.Marina.MarinaOwnerId == marinaOwnerId)
                .ToListAsync();

            return spots;
        }

        public async Task<Spot> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var spot = await LoadSpots()
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
            await _locationService.Create(location);
            spot.Location = location;

            return await Create(spot);
        }

        public async Task<Spot> UpdateSpotLocation(Spot spot, Location location)
        {
            if (spot.LocationId == null || spot.LocationId == 0)
            {
                await _locationService.Create(location);
                spot.Location = location;
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
