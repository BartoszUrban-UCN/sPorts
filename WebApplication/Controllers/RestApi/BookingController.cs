using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebApplication.Authorization;
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
        private readonly IAuthorizationService _authorizationService;

        public BookingController(IBookingService bookingService, IBoatService boatService, ISpotService spotService, IAuthorizationService authorizationService)
        {
            _bookingService = bookingService;
            _boatService = boatService;
            _spotService = spotService;
            _authorizationService = authorizationService;
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
            try
            {
                var booking = await _bookingService.GetSingle(id);
                return Ok(booking);
            }
            catch (BusinessException)
            {
                return NotFound();
            }
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
            try
            {
                var booking = await _bookingService.GetSingle(id);
                return Ok(booking.BookingLines);
            }
            catch (BusinessException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Validates the information input once again Creates or updates
        /// booking and bookingLine based on data from the wizard
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

            // Find boat & spot objects in db
            var boat = await _boatService.GetSingle(boatId);
            var spot = await _spotService.GetSingle(spotId);

            // Check whether the logged user owns the boat
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, boat, Operation.Book);

            if (!isAuthorized.Succeeded)
            {
                return Unauthorized();
            }

            // get booking from session if created before
            var booking = HttpContext.Session.Get<Booking>("Booking");

            // Check whether booking is consistent, and if not, reinitialize
            if (booking is null || booking.BookingReferenceNo == 0 || booking.BoatId != boatId)
            {
                booking = new Booking { BoatId = boatId };
                await _bookingService.Create(booking);
            }

            // If the spot fits the boat
            if (HelperMethods.DoesSpotFitBoat(boat, spot))
            {
                // And the selected dates are valid
                if (HelperMethods.AreDatesValid(startDate, endDate))
                {
                    // Next 5 lines make sure that no dates overlap in the
                    // booking's booking lines You cannot physically be in two
                    // places at the same time
                    bool areBookingLinesDatesValid = true;

                    foreach (BookingLine bookingLine in booking.BookingLines)
                        if (HelperMethods.AreDatesIntersecting(bookingLine.StartDate, bookingLine.EndDate, startDate, endDate))
                        {
                            areBookingLinesDatesValid = false;
                        }

                    // Finally, if all conditions are met
                    if (areBookingLinesDatesValid)
                    {
                        // Add bookingLine to the booking lines inside the booking
                        booking = _bookingService.CreateBookingLine(booking, startDate, endDate, spot);
                    }
                }
            }

            // store booking object in the session
            // don't yet know whether you rewrite value if you add it with the same key or if it needs to be removed first
            //HttpContext.Session.Remove("Booking");
            HttpContext.Session.Set("Booking", booking);

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

        /// <summary>
        /// Cancel spot / bookingline based on booking line id (marina owner)
        /// </summary>
        /// <param name="id">Booking line id</param>
        /// <returns>True or false whether if was cancelled or not</returns>
        [HttpPut("{id}/bookinglinecancellation")]
        public async Task<ActionResult<bool>> CancelSpotBookedByBookingLineId(int id)
        {
            var success = await _bookingService.CancelSpotBooked(id);
            return success;
        }

        /// <summary>
        /// Removes spot from your cart.
        /// Takes start date of a spot as a unique identifier.
        /// </summary>
        /// <param name="start"></param>
        /// <returns>Booking without the removed spot.</returns>
        [HttpDelete("RemoveBookingLine")]
        public async Task<ActionResult<Booking>> CartRemoveBookingLine([FromBody] string start)
        {
            var startDate = DateTime.Parse(start);

            var booking = HttpContext.Session.Get<Booking>("Booking");

            booking = _bookingService.CartRemoveBookingLine(booking, startDate);

            HttpContext.Session.Set("Booking", booking);

            return Ok(booking);
        }

        [HttpGet("InvalidBookings")]
        public async Task<ActionResult<IEnumerable<BookingLine>>> InvalidBookingLines()
        {
            var booking = HttpContext.Session.Get<Booking>("Booking");
            var invalidBookingLines = await _bookingService.InvalidBookingLines(booking);

            return Ok(invalidBookingLines);
        }

        /// <summary>
        /// Clears all spots from your cart.
        /// </summary>
        /// <returns>Booking wihtout booking lines / spots.</returns>
        [HttpDelete("ClearCart")]
        public async Task<ActionResult<Booking>> ClearCart()
        {
            HttpContext.Session.Set("Booking", new Booking());
            var booking = HttpContext.Session.Get<Booking>("Booking");
            return booking;
        }

        /// <summary>
        /// Create booking from your cart.
        /// Used for simulating concurrency 
        /// e.g. other user has booked your items ... how will the system react to that?
        /// </summary>
        /// <returns>Boolean whether it has successfully booked items from your cart.</returns>
        [HttpPost("CreateSampleBooking")]
        public async Task<ActionResult<bool>> CreateSampleBooking()
        {
            //Session
            var sessionBooking = HttpContext.Session.Get<Booking>("Booking");
            var booking = new Booking();

            if (sessionBooking != null)
            {
                booking = await _bookingService.LoadSpots(sessionBooking);
            }

            if (booking.BookingReferenceNo != 0)
            {
                try
                {
                    await _bookingService.SaveBooking(booking);
                }
                catch (Exception) { }
            }

            return true;
        }

        /// <summary>
        /// When user plans to stay longer in the marina, method will create new booking in the cart
        /// based on the attributes of a spot and marina user wants to stay longer in.
        /// Have NOT yet been tested.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns>View</returns>
        [HttpPost("{id}/AddTime")]
        public async Task<IActionResult> AddTime(int? id, [Bind("amount"), Range(1, 7)] byte amount)
        {
            var isAuthorized =
                User.IsInRole(RoleName.Administrator) ||
                User.IsInRole(RoleName.Manager) || User.IsInRole(RoleName.BoatOwner);

            if (isAuthorized)
            {
                try
                {
                    string message = "";
                    if (amount <= 0 || amount > 7)
                    {
                        message = "Please enter amount in days between 1 and 7 (including)";
                    }
                    else
                    {
                        var bookingLine = await _bookingService.AddTime(id, (int)amount);

                        if (bookingLine != null)
                        {
                            await CreateBookingLocally(bookingLine.Booking.BoatId, bookingLine.SpotId, bookingLine.StartDate.ToString(), bookingLine.EndDate.ToString());
                            return RedirectToAction("ShoppingCart", "BookingFlow");
                        }
                        else
                        {
                            message = "Someone else has booked the spot for that time period. Can NOT add specified time.";
                        }
                    }

                    return RedirectToAction("Details", "BookingLine", new { id = id, message = message });
                }
                catch
                {
                    throw;
                }
            }

            return Forbid();
        }
    }
}

