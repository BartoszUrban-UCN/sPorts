using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Models;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IBoatService _boatService;
        private readonly ISpotService _spotService;

        public BookingController(IBookingService bookingService, IBoatService boatService, ISpotService spotService)
        {
            _bookingService = bookingService;
            _boatService = boatService;
            _spotService = spotService;
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        /// <returns>List of bookings</returns>
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> Bookings()
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
        public async Task<ActionResult<Booking>> Booking(int id)
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
        public async Task<ActionResult<IEnumerable<Booking>>> BookingLines(int id)
        {
            var booking = await _bookingService.GetSingle(id);

            if (booking != null)
            {
                return Ok(booking.BookingLines);
            }
            return NotFound();
        }

        /// <summary>
        /// Creates booking and bookingLine based on data from the wizard
        /// </summary>
        /// <param name="boatId"></param>
        /// <param name="spotId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Booking in json format</returns>
        [Produces("application/json")]
        [HttpPost("createbookinglocally")]
        public async Task<ActionResult<Booking>> CreateBookingLocally(int boatId, int spotId, string start, string end)
        {
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            // find boat & spot objects in db
            var boat = await _boatService.GetSingle(boatId);
            var spot = await _spotService.GetSingle(spotId);

            // get booking from session if created before
            // else init booking
            var booking = HttpContext.Session.Get<Booking>("Booking");
            if (booking is null)
            {
                booking = new Booking { BoatId = boatId };
                await _bookingService.Create(booking);
            }

            // add bookingLine to the booking.BookingLines list
            booking = _bookingService.CreateBookingLine(booking, startDate, endDate, spot);

            // store booking object in the session
            // don't yet know whether you rewrite value if you add it with the same key or if it needs to be removed first
            //HttpContext.Session.Remove("Booking");
            HttpContext.Session.Add("Booking", booking);

            // hopefully serialization is not needed and returns booking in json format
            return Ok(booking);
        }

        /// <summary>
        /// Cancel booking based on booking id
        /// </summary>
        /// <param name="id">Booking id</param>
        /// <returns>True or false whether it was cancelled or not</returns>
        [HttpPut]
        public async Task<ActionResult<bool>> CancelBooking(int id)
        {
            await _bookingService.CancelBooking(id);
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


        [HttpDelete("RemoveBookingLine")]
        public async Task<ActionResult<Booking>> CartRemoveBookingLine([FromBody] int bookingLineId)
        {
            var bookingLine = await _bookingService.GetBookingLine(bookingLineId);

            var booking = HttpContext.Session.Get<Booking>("Booking");
            booking.BookingLines.RemoveAll(bookingLine => bookingLine.BookingLineId == bookingLineId);
            booking.TotalPrice = _bookingService.BookingCalculatePrice(booking.BookingLines);
            //booking = _bookingService.CartRemoveBookingLine(booking, bookingLineId);

            HttpContext.Session.Add("Booking", booking);
            return booking;
        }

        public async Task<IActionResult> ClearCart()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
