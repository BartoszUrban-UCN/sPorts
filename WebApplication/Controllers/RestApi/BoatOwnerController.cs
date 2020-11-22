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
        private readonly IBoatOwnerService _boatOwnerService;

        public BoatOwnerController(SportsContext context, IBoatOwnerService boatOwnerService)
        {
            _context = context;
            _boatOwnerService = boatOwnerService;
        }

        // GET: api/BoatOwner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoatOwner>>> BoatOwners()
        {
            return await _context.BoatOwners.ToListAsync();
        }

        // GET: api/BoatOwner/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoatOwner>> BoatOwner(int id)
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
        public async Task<IActionResult> BoatOwner(int id, BoatOwner boatOwner)
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> Bookings(int id)
        {
            try
            {
                var bookings = await _boatOwnerService.GetBookings(id);
                return Ok(bookings);
            }
            catch (BusinessException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> OngoingBookings(int id)
        {
            try
            {
                var ongoingBookings = await _boatOwnerService.GetOngoingBookings(id);
                return Ok(ongoingBookings);
            }
            catch (BusinessException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Boat>>> Boats(int id)
        {
            var boatOwnerWithBoats = await _context.BoatOwners.Include(b => b.Boats)
                                                        .ToListAsync();
            var boatOwner = boatOwnerWithBoats.Find(b => b.BoatOwnerId == id);

            if (boatOwner != null)
            {
                return Ok(boatOwner.Boats);
            }
            return NotFound();
        }

        private bool BoatOwnerExists(int id)
        {
            return _context.BoatOwners.Any(e => e.BoatOwnerId == id);
        }
    }
}