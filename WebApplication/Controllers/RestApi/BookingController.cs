using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly SportsContext _context;
        private readonly IBookingService _bookingService;
        private readonly IBookingConfirmationService _bookingConfirmationService;

        public BookingController(SportsContext context, IBookingService bookingService, IBookingConfirmationService bookingConfirmationService)
        {
            _context = context;
            _bookingService = bookingService;
            _bookingConfirmationService = bookingConfirmationService;
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
            var bookingLines = await _bookingService.GetBookingLines(id);

            if (bookingLines != null)
            {
                return Ok(bookingLines);
            }
            return NotFound();
        }

        [Produces("application/json")]
        [HttpGet("marinaowner")]
        public async Task<ActionResult<IEnumerable<BookingLine>>> GetBookingsByMarinaOwner()
        {
            // get logged marina owner
            int marinaOwnerId = 1;
            var marinaOwnerBookingLines = await _bookingConfirmationService.GetUnconfirmedBookingLines(marinaOwnerId);

            if (marinaOwnerBookingLines != null)
            {
                return Ok(marinaOwnerBookingLines);
            }

            return NotFound();
        }

        [HttpPut("confirmbookingline/{id}")]
        public async Task<ActionResult<bool>> ConfirmBookingLineById(int bookingLineId)
        {
            var success = await _bookingConfirmationService.ConfirmSpotBooked(bookingLineId);
            return success;
        }
    }
}
