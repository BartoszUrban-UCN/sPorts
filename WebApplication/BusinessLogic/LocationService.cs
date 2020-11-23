using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class LocationService : ServiceBase, ILocationService
    {
        public LocationService(SportsContext context) : base(context)
        { }

        // Persist a location to the database
        public async Task<int> Create(Location location)
        {
            // If location is null throw an error
            location.ThrowIfNull();

            // Add the location to the database and save the changes made
            await _context.AddAsync(location);

            // Return the newly created location's id
            return location.LocationId;
        }

        public async Task Delete(int? id)
        {
            // Find location in the database
            var location = await GetSingle(id);
            // Remove the location from the database
            _context.Locations.Remove(location);
        }

        public async Task<IEnumerable<Location>> GetAll()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == id);

            location.ThrowIfNull();

            return location;
        }

        public Location Update(Location location)
        {
            location.ThrowIfNull();

            _context.Update(location);

            return location;
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();

            return await _context.Locations.AnyAsync(l => l.LocationId == id);
        }
    }
}
