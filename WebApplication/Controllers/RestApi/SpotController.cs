using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApplication.Models;
using WebApplication.Data;
using System.Linq;
using System;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotsController : ControllerBase
    {
        private readonly SportsContext _context;

        public SpotsController(SportsContext context)
        {
            _context = context;
        }

        //GET: api/Spots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Spot>>> GetSpots()
        {
            return await _context.Spots.ToListAsync();
        }

        //Get: api/Spots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Spot>> GetSpots(int id)
        {
            var spot = await _context.Spots.FindAsync(id);

            if (spot == null)
            {
                return NotFound();
            }

            return spot;

        }

        //PUT: api/Spots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpot(int id, Spot spot)
        {
            if (id != spot.SpotId)
            {
                return BadRequest();
            }

            _context.Entry(spot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpotExists(id))
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

        //POST: api/Spots
        [HttpPost]
        public async Task<ActionResult<Spot>> PostSpot(Spot spot)
        {
            _context.Spots.Add(spot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpot", new { id = spot.SpotId }, spot);
        }

        // DELETE: api/Spots/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Spot>> DeleteSpot(int id)
        {
            var spot = await _context.Spots.FindAsync(id);
            if (spot == null)
            {
                return NotFound();
            }

            _context.Spots.Remove(spot);
            await _context.SaveChangesAsync();

            return spot;
        }
        private bool SpotExists(int id)
        {
            return _context.Spots.Any(s => s.SpotId == id);
        }
    }
}

