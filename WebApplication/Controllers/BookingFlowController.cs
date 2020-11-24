using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
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
        public async Task<IActionResult> Index(Booking booking)
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "Name", "Name");
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMarinaMap(string start, string end)
        {
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            //ViewBag.AvailableMarinas = _bookingService.GetAllAvailableSpotsCount(new List<int>() { 1, 2, 3, 4 }, 1, startDate, endDate);
            var marinas = await _bookingService.GetAll();
            //ViewBag.Marinas = await _bookingService.GetAll();
            return Json(marinas);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("Index")]
        //public async Task<IActionResult> IndexPost(Boat boat)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            boat = await _boatService.GetSingleByName(boat.Name);

        //            var booking = new Booking { BookingId = -1, BoatId = boat.BoatId, Boat = boat };
        //            var bookingLine = new BookingLine { BookingLineId = -1, Booking = booking };
        //            booking.BookingLines.Add(bookingLine);
        //            booking = await _bookingFormService.CreateBooking(booking);

        //            return View("ChoseDates", bookingLine);
        //        }
        //        catch (BusinessException)
        //        {
        //        }
        //    }

        //    ViewBag.Boat = new SelectList(await _boatService.GetAll(), "Name", "Name");
        //    return View();
        //}

        //public async Task<IActionResult> ChoseDates(BookingLine bookingLine)
        //{
        //    return View(bookingLine);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("ChoseDates")]
        //public async Task<IActionResult> ChoseDatesPost(BookingLine bookingLine, string startDate, string endDate)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var start = DateTime.Parse(startDate);
        //            var end = DateTime.Parse(endDate);
        //            BookingFormService.AreDatesValid(start, end);

        //            bookingLine.StartDate = start;
        //            bookingLine.EndDate = end;

        //            _bookingFormService.UpdateBookingLine(bookingLine);

        //            return View("ChoseMarina", bookingLine);
        //        }
        //        catch (BusinessException)
        //        {
        //        }
        //    }
        //    return View(bookingLine);
        //}

        //public async Task<IActionResult> ChoseDates(Booking booking)
        //{
        //    return View(booking);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChoseDatesPost(int bookingLineId, string startDate, string endDate)
        //{
        //    var bookingLine = await _bookingFormService.GetBookingLine(bookingLineId);
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var start = DateTime.Parse(startDate);
        //            var end = DateTime.Parse(endDate);
        //            BookingFormService.AreDatesValid(start, end);

        //            bookingLine.StartDate = start;
        //            bookingLine.EndDate = end;

        //            _bookingFormService.UpdateBookingLine(bookingLine);

        //            return View("ChoseMarina", bookingLine);
        //        }
        //        catch (BusinessException)
        //        {
        //        }
        //    }
        //    return View(bookingLine);
        //}

        //public async Task<IActionResult> ChoseMarina(BookingLine bookingLine)
        //{
        //    ViewBag.Marinas = new SelectList(await _marinaService.GetAll(), "MarinaId", "MarinaId");
        //    return View(bookingLine);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChoseMarinaPost(BookingLine bookingLine, Marina marina)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            return View("ChoseSpot", (bookingLine, marina));
        //        }
        //        catch (BusinessException)
        //        {
        //        }
        //    }
        //    return View(bookingLine);
        //}

        //public async Task<IActionResult> ChoseSpot(BookingLine bookingLine, Marina marina)
        //{
        //    ViewBag.Spots = _bookingFormService.GetAvailableSpots(marina.MarinaId, bookingLine.Booking.BoatId, bookingLine.StartDate, bookingLine.EndDate);
        //    return View(bookingLine);
        //}


        public async Task<IActionResult> ShoppingCart()
        {
            // for demonstration puroposes;
            var marinas = (System.Collections.Generic.List<Marina>) await _marinaService.GetAll();
            var spots = new System.Collections.Generic.List<Spot>();

            foreach(var marina in marinas)
            {
                spots.AddRange(marina.Spots);
            }

            return View("~/Views/Booking/ShoppingCart.cshtml", spots);
        }
    }
}
