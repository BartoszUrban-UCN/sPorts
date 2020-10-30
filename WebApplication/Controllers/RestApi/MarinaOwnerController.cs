using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarinaOwnerController : ControllerBase
    {
        private readonly SportsContext _context;

        public MarinaOwnerController(SportsContext context)
        {
            _context = context;
        }

        // GET: api/MarinaOwners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarinaOwner>>> GetMarinaOwners()
        {
            return await _context.MarinaOwners.Include(m => m.Spots).ToListAsync();
        }

        // GET: api/MarinaOwners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MarinaOwner>> GetMarinaOwner(int id)
        {
            var marinaOwner = await _context.MarinaOwners.FindAsync(id);

            if (marinaOwner == null)
            {
                return NotFound();
            }

            return marinaOwner;
        }

        // PUT: api/MarinaOwners/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMarinaOwner(int id, MarinaOwner marinaOwner)
        {
            if (id != marinaOwner.MarinaOwnerId)
            {
                return BadRequest();
            }

            _context.Entry(marinaOwner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarinaOwnerExists(id))
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

        // POST: api/MarinaOwners
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<MarinaOwner>> PostMarinaOwner(MarinaOwner marinaOwner)
        {
            _context.MarinaOwners.Add(marinaOwner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMarinaOwner", new { id = marinaOwner.MarinaOwnerId }, marinaOwner);
        }

        // DELETE: api/MarinaOwners/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MarinaOwner>> DeleteMarinaOwner(int id)
        {
            var marinaOwner = await _context.MarinaOwners.FindAsync(id);
            if (marinaOwner == null)
            {
                return NotFound();
            }

            _context.MarinaOwners.Remove(marinaOwner);
            await _context.SaveChangesAsync();

            return marinaOwner;
        }

        private bool MarinaOwnerExists(int id)
        {
            return _context.MarinaOwners.Any(e => e.MarinaOwnerId == id);
        }
    }
}
