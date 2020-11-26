using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class BookingFlowController : Controller
    {
        private readonly IBookingFormService _bookingFormService;
        private readonly IBookingService _bookingService;
        private readonly IBoatService _boatService;
        private readonly IMarinaService _marinaService;

        public BookingFlowController(IBookingFormService bookingFormService, IBookingService bookingService, IBoatService boatService, IMarinaService marinaService)
        {
            _bookingFormService = bookingFormService;
            _bookingService = bookingService;
            _boatService = boatService;
            _marinaService = marinaService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "Name", "Name");

            var booking = await _bookingFormService.CreateBooking();
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost(Booking booking)
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "Name", "Name");
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMarinaMap(string boatName, string start, string end)
        {
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            //var availableSpotsPerMarina = await _bookingFormService.GetAvailableSpotsPerMarina(await _marinaService.GetAll(), boatName, startDate, endDate);
            //var jsonString = HelperMethods.Serialize(availableSpotsPerMarina);

            var jsonString = HelperMethods.Serialize(await _marinaService.GetAll());

            return new JsonResult(jsonString);
        }

        public async Task<IActionResult> ShoppingCart()
        {
            // explicit load needed??
            //var booking = HttpContext.Session.Get<Booking>("Booking");

            var now = DateTime.Now;
            var then = now.AddDays(1);
            var marina = new Marina { Name = "Me gusta mucho" };
            var marina1 = new Marina { Name = "Aalborg Zoo" };
            var marina2 = new Marina { Name = "Underwater" };

            var spot = new Spot { Price = 12.3d };
            var spot1 = new Spot { Price = 5.5d };
            var spot2 = new Spot { Price = 8.9d };
            var spot3 = new Spot { Price = 3.3d };
            var spot4 = new Spot { Price = 8.5d };

            ViewData["Discount"] = 10;

            var bookingLine = new BookingLine { StartDate = now, EndDate = then, Spot = spot };
            var bookingLine1 = new BookingLine { StartDate = now, EndDate = then, Spot = spot1 };
            var bookingLine2 = new BookingLine { StartDate = now, EndDate = then, Spot = spot2 };
            var bookingLine3 = new BookingLine { StartDate = now, EndDate = then, Spot = spot3 };
            var bookingLine4 = new BookingLine { StartDate = now, EndDate = then, Spot = spot4 };

            var dict = new Dictionary<Marina, IEnumerable<BookingLine>>();
            dict.Add(key: marina, new List<BookingLine> { bookingLine, bookingLine1 });
            dict.Add(key: marina1, new List<BookingLine> { bookingLine2, bookingLine3 });
            dict.Add(key: marina2, new List<BookingLine> { bookingLine4 });

            return View("~/Views/Booking/ShoppingCart.cshtml", dict);
        }
    }
}
