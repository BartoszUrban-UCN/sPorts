using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoatOwnerController : ControllerBase
    {
        private readonly SportsContext _context;

        public BoatOwnerController(SportsContext context)
        {
            _context = context;
        }

        // GET: api/BoatOwner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoatOwner>>> GetBoatOwners()
        {
            return await _context.BoatOwners.ToListAsync();
        }

        // GET: api/BoatOwner/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoatOwner>> GetBoatOwner(int id)
        {
            var boatOwner = await _context.BoatOwners.FindAsync(id);

            if (boatOwner == null)
            {
                return NotFound();
            }

            return boatOwner;
        }

        // PUT: api/BoatOwner/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoatOwner(int id, BoatOwner boatOwner)
        {
            if (id != boatOwner.BoatOwnerId)
            {
                return BadRequest();
            }

            _context.Entry(boatOwner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoatOwnerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BoatOwner
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BoatOwner>> PostBoatOwner(BoatOwner boatOwner)
        {
            _context.BoatOwners.Add(boatOwner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBoatOwner", new { id = boatOwner.BoatOwnerId }, boatOwner);
        }

        // DELETE: api/BoatOwner/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BoatOwner>> DeleteBoatOwner(int id)
        {
            var boatOwner = await _context.BoatOwners.FindAsync(id);
            if (boatOwner == null)
            {
                return NotFound();
            }

            _context.BoatOwners.Remove(boatOwner);
            await _context.SaveChangesAsync();

            return boatOwner;
        }

        [HttpGet("{id}/bookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings(int id)
        {
            var boatWithBooking = _context.Boats.Include(b => b.Bookings);
            var boatList = await boatWithBooking.ToListAsync();
            var boat = boatList.Find(b => b.BoatId == id);

            if (boat != null)
            {
                var bookings = boat.Bookings;
                return Ok(bookings);
            }

            return NotFound(boat);
        }

        [HttpGet("{id}/ongbookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllOngoingBookings(int id)
        {
            try
            {
                var boatOwnersWithBoatsAndBookings = _context.BoatOwners.Include(bo => bo.Boats)
                                                            .ThenInclude(b => b.Bookings);
                var boatOwner = await boatOwnersWithBoatsAndBookings.FirstAsync(b => b.BoatOwnerId == id);
                var ongoingBookings = new BoatOwnerService(_context).OngoingBookings(boatOwner);
                
                return Ok(ongoingBookings);
            }
            catch (System.ArgumentNullException)
            {
                return NotFound();
            }
        }

        

        private bool BoatOwnerExists(int id)
        {
            return _context.BoatOwners.Any(e => e.BoatOwnerId == id);
        }
    }
}
