using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApplication.Models;
using WebApplication.Data;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarinasController : ControllerBase
    {
        private readonly SportsContext _context;

        public MarinasController(SportsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marina>>> GetMarinas()
        {
            return await _context.Marinas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MarinaOwner>> GetMarina(int id)
        {
            var marina = await _context.MarinaOwners.FindAsync(id);

            if (marina == null)
            {
                return NotFound();
            }

            return marina;
        }
    }
}
