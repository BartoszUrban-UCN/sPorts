using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarinaController : ControllerBase
    {
        private readonly SportsContext _context;

        public MarinaController(SportsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all the marinas in the database
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marina>>> GetMarinas()
        {
            //var marinaList = _context.Marinas.ToListAsync();

            //return Ok(await marinaList);
            return await _context.Marinas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarina(int id)
        {
            var marina = await _context.Marinas.FindAsync(id);

            if (marina != null)
            {
                return Ok(marina);
            }
            return NotFound();
        }
        [Produces("application/json")]
        [HttpGet("{id}/spots")]
        public async Task<IActionResult> GetMarinaSpots(int id)
        {
            var marinaWithSpots = _context.Marinas.Include(s => s.Spots);
            var marinaList = await marinaWithSpots.ToListAsync();
            var marina = marinaList.Find(m => m.MarinaId == id);

            if (marina != null)
            {
                var spots = marina.Spots;
                return Ok(spots);
            }
            return NotFound();
        }
    }
}