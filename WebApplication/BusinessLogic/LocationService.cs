using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class LocationService : ILocationService
    {
        private readonly SportsContext _context;

        public LocationService(SportsContext context)
        {
            _context = context;
        }

        public async Task<int> Create(Location location)
        {
            _context.Add(location);
            await _context.SaveChangesAsync();
            return location.LocationId;
        }

        public async Task Delete(int? id)
        {
            var location = await _context.Locations.FindAsync(id);
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Location>> GetAll()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location> GetSingle(int? id)
        {
            return await _context.Locations
                .FindAsync(id);
        }

        public async Task<Location> Update(Location location)
        {
            _context.Update(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public Task<bool> Exists(int? id)
        {
            return _context.Locations.AnyAsync(l => l.LocationId == id);
        }
    }
}
