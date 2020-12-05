using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Query;

namespace WebApplication.BusinessLogic
{
    public class MarinaService : ServiceBase, IMarinaService
    {
        private readonly ILocationService _locationService;

        public MarinaService(SportsContext context, ILocationService locationService) : base(context)
        {
            _locationService = locationService;
        }

        public async Task<int> Create(Marina marina)
        {
            marina.ThrowIfNull();

            await _context.AddAsync(marina);

            return marina.MarinaId;
        }

        public async Task<int> CreateWithLocation(Marina marina, Location location)
        {
            marina.ThrowIfNull();
            location.ThrowIfNull();

            // Create location for marina
            await _locationService.Create(location);

            // Create marina
            await Create(marina);

            // Assign location to marina
            marina.Location = location;

            return marina.MarinaId;
        }

        public async Task<int> CreateLocationForMarina(Marina marina, Location location)
        {
            marina.ThrowIfNull();
            location.ThrowIfNull();
            // Create location for marina and take the Id
            await _locationService.Create(location);

            // Assign location to marina
            marina.Location = location;

            return marina.MarinaId;
        }

        // Delete a marina as well as its related location, if it has one
        // Checks whether id is valid and whether a marina with that id exists
        public async Task Delete(int? id)
        {
            // Find marina in the database by a given id
            var marina = await GetSingle(id);

            // Remove the marina's location from the database and remove associations, if it has one
            if (marina.LocationId is not null)
            {
                await _locationService.Delete(marina.LocationId);
            }

            // Remove the marina from the database
            _context.Marinas.Remove(marina);
        }

        public async Task<Marina> DeleteMarinaLocation(Marina marina)
        {
            marina = await GetSingle(marina.MarinaId);
            marina.ThrowIfNull();
            marina.LocationId.ThrowIfInvalidId();
            await _locationService.Delete(marina.LocationId);
            return marina;
        }

        public async Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();
            return await _context.Marinas.AnyAsync(m => m.MarinaId == id);
        }

        public async Task<bool> NotExists(int? id)
        {
            id.ThrowIfInvalidId();

            return !await _context.Marinas.AnyAsync(m => m.MarinaId == id);
        }
        // Get all Marinas and load their related entities also
        private IIncludableQueryable<Marina, Location> LoadMarinas()
        {
            var marinas = _context.Marinas
                .Include(marina => marina.Address)
                .Include(marina => marina.MarinaOwner)
                    .ThenInclude(marinaOwner => marinaOwner.Person)
                .Include(marina => marina.Location)
                .Include(marina => marina.Spots)
                    .ThenInclude(spot => spot.BookingLines)
                .Include(marina => marina.Spots)
                    .ThenInclude(spot => spot.Location);
            
            return marinas;
        }
        public async Task<IEnumerable<Marina>> GetAll()
        {
            var marinas = await LoadMarinas()
                                    .ToListAsync();

            foreach (var marina in marinas)
                if (marina.Location is null && marina.Spots.Count > 0 && marina.Spots.Any(spot => spot.LocationId != null))
                    CalculateMarinaLocation(marina);

            return marinas;
        }

        public async Task<IEnumerable<Marina>> GetAll(int marinaOwnerId)
        {
            var marinas = await LoadMarinas()
                                    .Where(p => p.MarinaOwner.PersonId == marinaOwnerId)
                                    .ToListAsync();
            
            foreach (var marina in marinas)
                if (marina.Location is null && marina.Spots.Count > 0 && marina.Spots.Any(spot => spot.LocationId != null))
                    CalculateMarinaLocation(marina);

            return marinas;
        }

        public async Task<Marina> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();
            
            var marina = await LoadMarinas()
                .FirstOrDefaultAsync(marina => marina.MarinaId == id);

            marina.ThrowIfNull();

            if (marina.Location is null && marina.Spots.Count > 0 && marina.Spots.Any(spot => spot.LocationId != null))
            {
                CalculateMarinaLocation(marina);
            }

            return marina;
        }

        public Marina Update(Marina marina)
        {
            marina.ThrowIfNull();

            _context.Update(marina);

            return marina;
        }

        public Marina UpdateMarinaLocation(Marina marina, Location location)
        {
            marina.ThrowIfNull();
            location.ThrowIfNull();

            _locationService.Update(location);

            return marina;
        }

        /// <summary>
        /// Calculate a marina's location based on the locations that its spots have, if it has any.
        /// </summary>
        /// <remarks>
        /// Uses a algorithm in order to determine the smallest enclosing circle of a number of points.
        /// </remarks>
        /// <param name="marina"></param>
        public static void CalculateMarinaLocation(Marina marina)
        {
            if (marina is not null)
                if (MarinaSpotsHaveLocations(marina))
                {
                    var locations = new List<Point>();

                    // Verify for spots with locations once again
                    // (When the method gets called it is made sure that the spots inside have a valid location, but let's verify again)
                    marina.Spots
                        .FindAll(spot => spot.LocationId.IsValidId())
                        .ForEach(spot => locations.Add(new Point(spot.Location.Latitude, spot.Location.Longitude)));

                    Circle circle = SmallestEnclosingCircle.MakeCircle(locations);

                    if (circle.r * 8000 < 10000)
                        circle.r = 10;

                    Location marinaLocation = new Location
                    {
                        Latitude = circle.c.x,
                        Longitude = circle.c.y,
                        Radius = circle.r,
                    };

                    marina.Location = marinaLocation;
                }
        }

        public static bool MarinaSpotsHaveLocations(Marina marina)
        {
            marina.ThrowIfNull();
            foreach (Spot spot in marina.Spots)
                if (spot.LocationId.IsValidId())
                    return true;

            return false;
        }
    }
}
