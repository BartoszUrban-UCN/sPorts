using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic.Shared;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace WebApplication.BusinessLogic
{
    public class BoatService : ServiceBase, IBoatService
    {
        public BoatService(SportsContext context) : base(context)
        { }

        public async Task<int> Create(Boat boat)
        {
            boat.ThrowIfNull();

            await _context.AddAsync(boat);

            return boat.BoatId;
        }

        public async Task<Boat> GetSingle(int? id)
        {
            id.ThrowIfInvalidId();

            var boat = await _context.Boats
                                        .Include(b => b.Bookings)
                                            .ThenInclude(b => b.BookingLines)
                                        .FirstOrDefaultAsync(b => b.BoatId == id);

            boat.ThrowIfNull();

            return boat;
        }

        public async Task<Boat> GetSingleByName(string name)
        {
            name.ThrowIfNull();

            var boat = await _context.Boats
                .Include(b => b.Bookings)
                    .ThenInclude(b => b.BookingLines)
                .FirstOrDefaultAsync(b => b.Name == name);

            boat.ThrowIfNull();

            return boat;
        }

        public async Task<IEnumerable<Boat>> GetAll()
        {
            var boats = await _context.Boats
                .Include(boat => boat.Bookings)
                .Include(boat => boat.BoatOwner)
                .ToListAsync();
            return boats;
        }

        public Boat Update(Boat boat)
        {
            _context.Update(boat);

            return boat;
        }

        public async Task Delete(int? id)
        {
            var boat = await GetSingle(id);
            _context.Boats.Remove(boat);
        }

        public Task<bool> Exists(int? id)
        {
            id.ThrowIfInvalidId();
            return _context.Boats.AnyAsync(b => b.BoatId == id);
        }
    }
}
