using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class BookingFlow : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IBoatService _boatService;

        public BookingFlow(IBookingService bookingService, IBoatService boatService)
        {
            _bookingService = bookingService;
            _boatService = boatService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Boat boat)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    boat = await _boatService.GetSingleByName(boat.Name);
                    var booking = new Booking { BoatId = boat.BoatId, Boat = boat };
                    var bookingLine = new BookingLine { Booking = booking };
                    return View("ChoseDates", bookingLine);
                }
                catch (BusinessException)
                {
                }
            }

            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "Name", "Name");
            return View();
        }

        public async Task<IActionResult> ChoseDates(BookingLine bookingLine)
        {
            return View(bookingLine);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChoseDates(BookingLine bookingLine)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            return View("ChoseMarina", bookingLine);
        //        }
        //        catch (BusinessException)
        //        {
        //        }
        //    }
        //    return View(bookingLine);
        //}

        public async Task<IActionResult> ChoseMarina(BookingLine bookingLine)
        {
            if (ModelState.IsValid)
            {
                try
                {
                }
                catch (BusinessException)
                {
                }
            }

            return View(bookingLine);
        }
    }
}
