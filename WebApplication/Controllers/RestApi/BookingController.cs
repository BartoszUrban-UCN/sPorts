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
    public class BookingController : ControllerBase
    {
        private readonly SportsContext _context;

        public BookingController(SportsContext context)
        {
            _context = context;
        }

        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings.ToListAsync();
        }

        [Produces("application/json")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                return Ok(booking);
            }
            return NotFound();
        }

        [Produces("application/json")]
        [HttpGet("{id}/lines")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingLines(int id)
        {
            var bookingWithBookingLines = _context.Bookings.Include(l => l.BookingLines);
            var bookingList = await bookingWithBookingLines.ToListAsync();
            var booking = bookingList.Find(b => b.BookingId == id);

            if (booking != null)
            {
                var bookingLines = booking.BookingLines;

                return Ok(bookingLines);
            }
            return NotFound();
        }
    }
}