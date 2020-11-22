using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic.Shared;

namespace WebApplication.BusinessLogic
{
    public class MarinaService : IMarinaService
    {
        private readonly SportsContext _context;
        private readonly ILocationService _locationService;

        public MarinaService(SportsContext context, ILocationService locationService)
        {
            // if (context == null)
            //     throw new BusinessException("MarinaService", "The context argument was null.");

            // if (locationService == null)
            //     throw new BusinessException("MarinaService", "The locationService argument was null.");

            _context = context;
            _locationService = locationService;
        }

        public async Task<int> Create(Marina marina)
        {
            if (marina is not null)
            {
                _context.Marinas.Add(marina);

                await _context.SaveChangesAsync();
            }

            return marina.MarinaId;
        }

        public async Task<int> CreateWithLocation(Marina marina, Location location)
        {
            if (marina is not null)
                if (location is not null)
                {
                    // Create location for marina and take the Id
                    var locationIdForMarina = await _locationService.Create(location);

                    // Create marina
                    await Create(marina);

                    // Assign location to marina
                    marina.LocationId = locationIdForMarina;

                    await _context.SaveChangesAsync();

                    return marina.MarinaId;
                }

            return marina.MarinaId;
        }

        // Delete a marina as well as its related location, if it has one
        // Checks whether id is valid and whether a marina with that id exists
        public async Task Delete(int? id)
        {
            if (id.IsValidId())
                if (await Exists(id))
                {
                    // Find marina in the database by a given id
                    var marina = await GetSingle(id);

                    // Remove the marina's location from the database and remove associations, if it has one
                    if (marina.LocationId is not null)
                        await DeleteMarinaLocation(marina);

                    // Remove the marina from the database
                    _context.Marinas.Remove(marina);

                    // Save the changes made
                    await _context.SaveChangesAsync();
                }
        }

        public async Task DeleteMarinaLocation(Marina marina)
        {
            if (marina is not null)
                if (marina.LocationId is not null)
                {
                    var locationId = marina.LocationId;

                    marina.Location = null;
                    marina.LocationId = null;

                    await _locationService.Delete(locationId);
                }
        }

        public async Task<bool> Exists(int? id)
        {
            if (id.IsValidId())
                return await _context.Marinas.AnyAsync(m => m.MarinaId == id);

            return false;
        }

        public async Task<bool> NotExists(int? id)
        {
            id.ThrowIfInvalidId();

            return !await _context.Marinas.AnyAsync(m => m.MarinaId == id);
        }

        // Get all Marinas and load their related entities also
        public async Task<IEnumerable<Marina>> GetAll()
        {
            var marinas = await _context.Marinas
                .Include(marina => marina.Address)
                .Include(marina => marina.MarinaOwner)
                .Include(marina => marina.Location)
                .Include(marina => marina.Spots)//.Where(spot => spot.LocationId != null))
                    .ThenInclude(spot => spot.Location)
                .ToListAsync();

            return marinas;
        }

        public async Task<Marina> GetSingle(int? id)
        {
            if (id.IsValidId())
            {
                var marina = await _context.Marinas
                    .Include(marina => marina.Address)
                    .Include(marina => marina.MarinaOwner)
                    .Include(marina => marina.Location)
                    .Include(marina => marina.Spots)//.Where(spot => spot.LocationId != null))
                        .ThenInclude(spot => spot.Location)
                    .FirstOrDefaultAsync(marina => marina.MarinaId == id);

                if (marina is not null)
                    return marina;
            }

            throw new BusinessException("Error", "Not Found");
        }

        public async Task<Marina> Update(Marina marina)
        {
            if (marina is not null)
            {
                _context.Marinas.Update(marina);

                await _context.SaveChangesAsync();
            }

            return marina;
        }

        public async Task<Marina> UpdateMarinaLocation(Marina marina, Location location)
        {
            if (marina is not null)
                if (location is not null)
                {
                    await _locationService.Update(location);

                    await _context.SaveChangesAsync();
                }

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
                    IList<Point> locations = new List<Point>();

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
            foreach (Spot spot in marina.Spots)
                if (spot.LocationId.IsValidId())
                    return true;

            return false;
        }
    }
}
