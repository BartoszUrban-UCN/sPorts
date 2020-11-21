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
            // if (context == null)
            //     throw new BusinessException("LocationService", "The context argument was null.");

            _context = context;
        }

        public async Task<int> Create(Location location)
        {
            // TODO Might not be necesary
            // TODO Remove if the caller already checks for null
            if (location == null)
            {
                throw new BusinessException("Create", "Location is null.");
            }

            _context.Locations.Add(location);
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Create", "The Location was not created.");

                return location.LocationId;
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
            var location = await GetSingle(id);
            _context.Locations.Remove(location);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Delete", "Couldn't delete the Location.");
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

        public async Task<IEnumerable<Location>> GetAll()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location> GetSingle(int? id)
        {
            if (id < 0)
                throw new BusinessException("GetSingle", "The id was negative.");

            var location = await _context.Locations.FindAsync(id);
        
            if (location == null)
                throw new BusinessException("GetSingle", $"Didn't find Location with id {id}");

            return location;
        }

        public async Task<Location> Update(Location location)
        {
            _context.Locations.Update(location);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Update", "The Location was not updated.");

                return location;
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

            return _context.Locations.AnyAsync(l => l.LocationId == id);
        }
    }
}
