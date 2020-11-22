﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
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

        // Persist a location to the database
        public async Task<int> Create(Location location)
        {
            // If location is null throw an error
            location.ThrowIfNull();

            // Add the location to the database and save the changes made
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            // Return the newly created location's id
            return location.LocationId;
        }

        public async Task Delete(int? id)
        {
            // If id is null or negative, throw an error
            id.ThrowIfInvalidId();

            // Find location in the database
            var location = await GetSingle(id);
            // Remove the location from the database
            _context.Locations.Remove(location);

            // Save the changes made
            await _context.SaveChangesAsync();
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

        public async Task<Location> Update(Location location)
        {
            location.ThrowIfNull();

            _context.Locations.Update(location);
            await _context.SaveChangesAsync();

            return location;
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();

            return await _context.Locations.AnyAsync(l => l.LocationId == id);
        }
    }
}
