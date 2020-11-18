using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace WebApplication.BusinessLogic
{
    public class MarinaService : IMarinaService
    {
        private readonly SportsContext _context;
        private readonly ILocationService _locationService;

        public MarinaService(SportsContext context, ILocationService locationService)
        {
            _context = context;
            _locationService = locationService;
        }

        public async Task<int> Create(Marina marina)
        {
            _context.Marinas.Add(marina);

            await _context.SaveChangesAsync();

            return marina.MarinaId;
        }

        public async Task<int> CreateWithLocation(Marina marina, Location location)
        {
            // Create location for marina and take the Id
            var locationForMarinaId = await _locationService.Create(location);
            // Create marina
            await Create(marina);
            // Assign location to marina
            marina.LocationId = locationForMarinaId;

            await _context.SaveChangesAsync();

            return marina.MarinaId;
        }

        public async Task Delete(int? id)
        {
            var marina = await _context.Marinas.FindAsync(id);
            _context.Marinas.Remove(marina);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMarinaLocation(Marina marina)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Exists(int? id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Marina>> GetAll()
        {
            // Get all marinas, and if a marina does not have a location, load the marina's spots that do have a location
            var sportsContext = _context.Marinas
                .Include(marina => marina.Address)
                .Include(marina => marina.MarinaOwner)
                .Include(marina => marina.Location)
                .Include(marina => marina.Spots.Where(spot => spot.LocationId != null))
                    .ThenInclude(spot => spot.Location)
                .Where(marina => marina.LocationId == null);

            return await sportsContext.ToListAsync();
        }

        public async Task<Marina> GetSingle(int? id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Marina> Update(Marina objectToUpdate)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Spot> UpdateMarinaLocation(Marina marina, Location location)
        {
            throw new System.NotImplementedException();
        }

        //
        public static Circle CalculateMarinaLocation(Marina marina)
        {
            IList<Point> locations = new List<Point>();

            marina.Spots
                .FindAll(spot => spot.LocationId != null)
                .ForEach(spot => locations.Add(new Point(spot.Location.XLatitude, spot.Location.YLongitude)));

            Circle circle = SmallestEnclosingCircle.MakeCircle(locations);

            Location marinaLocation = new Location
            {
                XLatitude = circle.c.x,
                YLongitude = circle.c.y
            };

            marina.Location = marinaLocation;

            return circle;
        }
    }
}
