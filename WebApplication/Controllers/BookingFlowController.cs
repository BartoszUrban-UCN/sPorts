using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
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
            // for demonstration puroposes;
            var marinas = (System.Collections.Generic.List<Marina>)await _marinaService.GetAll();
            var spots = new System.Collections.Generic.List<Spot>();

            foreach (var marina in marinas)
            {
                spots.AddRange(marina.Spots);
            }

            return View("~/Views/Booking/ShoppingCart.cshtml", spots);
        }
    }
}
