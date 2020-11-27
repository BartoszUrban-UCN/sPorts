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
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "BoatId", "Name");

            var booking = await _bookingFormService.CreateBooking();
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost(Booking booking)
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "BoatId", "Name");
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMarinaMap(string boat, string start, string end)
        {
            var boatId = int.Parse(boat);
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            //var availableSpotsPerMarina = await _bookingFormService.GetAvailableSpotsPerMarina(await _marinaService.GetAll(), boatName, startDate, endDate);
            //var jsonString = HelperMethods.Serialize(availableSpotsPerMarina);

            var jsonString = HelperMethods.Serialize(await _marinaService.GetAll());

            return new JsonResult(jsonString);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpotMap(string boat, string start, string end, string marina)
        {
            var boatId = int.Parse(boat);
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);
            var marinaId = int.Parse(marina);

            //var availableSpotsPerMarina = await _bookingFormService.GetAvailableSpotsPerMarina(await _marinaService.GetAll(), boatName, startDate, endDate);
            //var jsonString = HelperMethods.Serialize(availableSpotsPerMarina);

            var jsonString = HelperMethods.Serialize(await _bookingFormService.GetAvailableSpots(marinaId, boatId, startDate, endDate));

            return new JsonResult(jsonString);
        }

        public async Task<IActionResult> ShoppingCart()
        {
            // explicit load needed??
            //var booking = HttpContext.Session.Get<Booking>("Booking");
            //var validBooking = _bookingService.ValidateShoppingCart(booking);

            //bool hasChanged = false;
            //if (!validBooking.Equals(booking))
            //{
            //    hasChanged = true;
            //}

            //ViewData["BookingChange"] = hasChanged;

            var now = DateTime.Now;
            var then = now.AddDays(1);

            var person = new Person { FirstName = "Jonny", Email = "jimmyjackson@gmail.com" };
            var person1 = new Person { FirstName = "Elton", Email = "eltonjohn@gmail.com" };

            var marinaOwner = new MarinaOwner { Person = person };

            var marina = new Marina { MarinaId = 1, Name = "Me gusta mucho", MarinaOwner = marinaOwner };
            var marina1 = new Marina { MarinaId = 2, Name = "Aalborg Zoo", MarinaOwner = marinaOwner };
            var marina2 = new Marina { MarinaId = 3, Name = "Underwater", MarinaOwner = marinaOwner };

            var boatOwner = new BoatOwner { Person = person };

            var boat = new Boat { Name = "Mama Destroyer", BoatOwner = boatOwner };

            var spot = new Spot { SpotId = 1, Price = 12.3d, Marina = marina, SpotNumber = 5 };
            var spot1 = new Spot { SpotId = 2, Price = 5.5d, Marina = marina, SpotNumber = 2 };
            var spot2 = new Spot { SpotId = 3, Price = 8.9d, Marina = marina, SpotNumber = 3 };
            var spot3 = new Spot { SpotId = 4, Price = 3.3d, Marina = marina1, SpotNumber = 6 };
            var spot4 = new Spot { SpotId = 5, Price = 8.5d, Marina = marina2, SpotNumber = 1 };

            var booking1 = new Booking
            {
                Boat = boat,
            };

            await _bookingService.Create(booking1);
            booking1 = _bookingService.CreateBookingLine(booking1, now, then, spot);
            booking1 = _bookingService.CreateBookingLine(booking1, now, then, spot1);
            booking1 = _bookingService.CreateBookingLine(booking1, now, then, spot2);
            booking1 = _bookingService.CreateBookingLine(booking1, now, then, spot3);
            booking1 = _bookingService.CreateBookingLine(booking1, now, then, spot4);

            HttpContext.Session.Add<Booking>("Booking", booking1);

            var marinaBLineDict = _bookingService.FilterLinesByMarina(booking1);
            ViewData["MarinaBLineDict"] = marinaBLineDict;

            return View(booking1);
        }
    }
}
