using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

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
            if (marina == null)
            {
                throw new BusinessException("Create", "Marina object is null.");
            }

            _context.Marinas.Add(marina);
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Create", "The Marina was not created.");

                return marina.MarinaId;
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

        public async Task<int> CreateWithLocation(Marina marina, Location location)
        {
            // Create location for marina and take the Id
            var locationForMarinaId = await _locationService.Create(location);
            // Create marina
            await Create(marina);
            // Assign location to marina
            marina.LocationId = locationForMarinaId;

            return marina.MarinaId;
        }

        public async Task Delete(int? id)
        {
            var marina = await GetSingle(id);
            _context.Marinas.Remove(marina);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Delete", "Couldn't delete the Marina.");
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

        public async Task DeleteMarinaLocation(Marina marina)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Exists(int? id)
        {
            if (id < 0)
                throw new BusinessException("Exists", "The id is negative.");

            return await _context.Marinas.AnyAsync(m => m.MarinaId == id);
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
            if (id == null)
                throw new BusinessException("GetSingle", "Id is null.");

            if (id < 0)
                throw new BusinessException("GetSingle", "The id was negative.");

            var marina = await _context.Marinas
                .Include(marina => marina.Address)
                .Include(marina => marina.MarinaOwner)
                .Include(marina => marina.Location)
                .Include(marina => marina.Spots)//.Where(spot => spot.LocationId != null))
                    .ThenInclude(spot => spot.Location)
                .FirstOrDefaultAsync(marina => marina.MarinaId == id);

            if (marina == null)
                throw new BusinessException("GetSingle", $"Didn't find Marina with id {id}");

            return marina;
        }

        public async Task<Marina> Update(Marina marina)
        {
            _context.Marinas.Update(marina);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                    throw new BusinessException("Update", "The Marina was not updated.");

                return marina;
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

        public async Task<Spot> UpdateMarinaLocation(Marina marina, Location location)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Calculate a marina's location based on the locations that its spots have, if it has any.
        /// </summary>
        /// <remarks>
        /// Uses a mathematical algorithm in order to determine the smallest enclosing circle of a number of points.
        /// </remarks>
        /// <param name="marina"></param>
        public static void CalculateMarinaLocation(Marina marina)
        {
            IList<Point> locations = new List<Point>();

            // Verify for spots with locations once again
            // (When the method gets called it is made sure that the spots inside have a valid location, but let's verify again)
            marina.Spots
                .FindAll(spot => spot.LocationId != null)
                .ForEach(spot => locations.Add(new Point(spot.Location.Latitude, spot.Location.Longitude)));

            Circle circle = SmallestEnclosingCircle.MakeCircle(locations);

            if (circle.r * 8000 < 10000)
            {
                circle.r = 10;
            }

            Location marinaLocation = new Location
            {
                Latitude = circle.c.x,
                Longitude = circle.c.y,
                Radius = circle.r,
            };

            marina.Location = marinaLocation;
        }
    }
}
