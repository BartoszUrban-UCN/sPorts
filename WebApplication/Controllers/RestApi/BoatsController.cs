using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoatsController : ControllerBase
    {
        private readonly SportsContext _context;

        public BoatsController(SportsContext context)
        {
            _context = context;
        }

        // GET: api/Boats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Boat>>> GetBoats()
        {
            return await _context.Boats.ToListAsync();
        }

        // GET: api/Boats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Boat>> GetBoat(int id)
        {
            var boat = await _context.Boats.FindAsync(id);

            if (boat == null)
            {
                return NotFound();
            }

            return boat;
        }

        // PUT: api/Boats/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoat(int id, Boat boat)
        {
            if (id != boat.BoatId)
            {
                return BadRequest();
            }

            _context.Entry(boat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoatExists(id))
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

        // POST: api/Boats
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Boat>> PostBoat(Boat boat)
        {
            _context.Boats.Add(boat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBoat", new { id = boat.BoatId }, boat);
        }

        // DELETE: api/Boats/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Boat>> DeleteBoat(int id)
        {
            var boat = await _context.Boats.FindAsync(id);
            if (boat == null)
            {
                return NotFound();
            }

            _context.Boats.Remove(boat);
            await _context.SaveChangesAsync();

            return boat;
        }

        private bool BoatExists(int id)
        {
            return _context.Boats.Any(e => e.BoatId == id);
        }
    }
}
