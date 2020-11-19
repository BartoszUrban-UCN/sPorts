using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        /// <returns>List of bookings</returns>
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var bookings = await _bookingService.GetAll();
            return Ok(bookings);
        }

        /// <summary>
        /// Get single booking based on id
        /// </summary>
        /// <param name="id">Booking id</param>
        /// <returns>Booking</returns>
        [Produces("application/json")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _bookingService.GetSingle(id);

            if (booking != null)
            {
                return Ok(booking);
            }
            return NotFound();
        }

        /// <summary>
        /// Get Booking lines of a booking
        /// </summary>
        /// <param name="id">Booking id</param>
        /// <returns>List of booking lines of a specified booking</returns>
        [Produces("application/json")]
        [HttpGet("{id}/lines")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingLines(int id)
        {
            var booking = await _bookingService.GetSingle(id);

            if (booking != null)
            {
                return Ok(booking.BookingLines);
            }
            return NotFound();
        }

        /// <summary>
        /// Get bookings by marina owner
        /// </summary>
        /// <returns>List of logged in marina owner bookings</returns>
        [Produces("application/json")]
        [HttpGet("marinaowner")]
        public async Task<ActionResult<IEnumerable<BookingLine>>> GetBookingsByMarinaOwner()
        {
            // get logged marina owner
            // var marinaOwner = await _marinaOwnerService.GetSingle(int loggedMarinaOwnerId);
            // var bookingLines = await _bookingService.GetBookingLinesByMarinaOwner(marinaOwner.MarinaOwnerId);

            int marinaOwnerId = 1;
            var marinaOwnerBookingLines = await _bookingService.GetBookingLinesByMarinaOwner(marinaOwnerId);

            if (marinaOwnerBookingLines != null)
            {
                return Ok(marinaOwnerBookingLines);
            }

            return NotFound();
        }

        /// <summary>
        /// Cancel booking based on booking id
        /// </summary>
        /// <param name="id">Booking id</param>
        /// <returns>True or false whether it was cancelled or not</returns>
        [HttpPut]
        public async Task<ActionResult<bool>> Cancel(int id)
        {
            var success = await _bookingService.CancelBooking(id);
            return Ok();
        }

        /// <summary>
        /// Confirm bookingline based on booking line id
        /// </summary>
        /// <param name="id">Booking line id</param>
        /// <returns>True or false whether if was confirmed or not</returns>
        [HttpPut("{id}/bookinglineconfirmation")]
        public async Task<ActionResult<bool>> ConfirmBookingLineById(int id)
        {
            var success = await _bookingService.ConfirmSpotBooked(id);
            return success;
        }
    }
}
