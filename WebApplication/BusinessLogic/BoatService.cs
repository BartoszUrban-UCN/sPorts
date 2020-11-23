using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public class BoatService : ServiceBase, IBoatService
    {
        public BoatService(SportsContext context) : base(context)
        {
        }

        public async Task<int> Create(Boat boat)
        {
            // TODO Might not be necesary
            // TODO Remove if the caller already checks for null
            if (boat == null)
            {
                throw new BusinessException("Create", "Boat object is null.");
            }

            _context.Boats.Add(boat);
            return boat.BoatId;
        }

        public async Task<Boat> GetSingle(int? id)
        {
            if (id == null)
                throw new BusinessException("GetSingle", "Id is null.");

            if (id < 0)
                throw new BusinessException("GetSingle", "The id was negative.");

            var boat = await _context.Boats
                                        .Include(b => b.Bookings)
                                            .ThenInclude(b => b.BookingLines)
                                        .FirstOrDefaultAsync(b => b.BoatId == id);

            if (boat == null)
                throw new BusinessException("GetSingle", $"Didn't find Boat with id {id}");

            return boat;
        }

        public async Task<Boat> GetSingleByName(string name)
        {
            if (name == null)
                throw new BusinessException("GetSingle", "Name is null.");

            var boat = await _context.Boats
                                        .Include(b => b.Bookings)
                                            .ThenInclude(b => b.BookingLines)
                                        .FirstOrDefaultAsync(b => b.Name == name);

            if (boat == null)
                throw new BusinessException("GetSingle", $"Didn't find Boat with name {name}");

            return boat;
        }

        public async Task<IEnumerable<Boat>> GetAll()
        {
            var boats = await _context.Boats
                                        .Include(b => b.Bookings)
                                        .ToListAsync();
            return boats;
        }

        public async Task<Boat> Update(Boat boat)
        {
            _context.Boats.Update(boat);
            return boat;
        }

        public async Task Delete(int? id)
        {
            var boat = await GetSingle(id);
            _context.Boats.Remove(boat);
        }

        public Task<bool> Exists(int? id)
        {
            if (id < 0)
                throw new BusinessException("Exists", "The id is negative.");

            return _context.Boats.AnyAsync(b => b.BoatId == id);
        }
    }
}
