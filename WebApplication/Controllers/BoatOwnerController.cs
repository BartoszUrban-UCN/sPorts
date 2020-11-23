using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BoatOwnerController : Controller
    {
        private readonly IBoatOwnerService _boatOwnerService;
        public BoatOwnerController(IBoatOwnerService boatOwnerService)
        {
            _boatOwnerService = boatOwnerService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _boatOwnerService.GetAll();
            return View(result);
        }

        public async Task<IActionResult> GetBookings(int? id)
        {
            try
            {
                var bookings = await _boatOwnerService.GetBookings(id);
                return View("~/Views/Booking/Index.cshtml", bookings);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> GetOngoingBookings(int? id)
        {
            try
            {
                var ongoingBookings = await _boatOwnerService.GetOngoingBookings(id);
                return View("~/Views/Booking/Index.cshtml", ongoingBookings);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        public IActionResult GetBookingLines(int? id)
        {
            return RedirectToAction("GetBookingLines", "Booking", new { id = id });
        }

        public IActionResult Cancel(int? id)
        {
            return RedirectToAction("Cancel", "Booking", new { id = id });
        }

        public IActionResult GetOngoingBookingLines(int? id)
        {
            return RedirectToAction("GetOngoingBookingLines", "Booking", new { id = id });
        }

        public async Task<IActionResult> GetBoats(int? id)
        {
            try
            {
                var boats = await _boatOwnerService.GetBoats(id);
                return View("~/Views/Boats/Index.cshtml", boats);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var boatOwner = await _boatOwnerService.GetSingle(id);

                ViewData["TotalSpent"] = _boatOwnerService.MoneySpent(boatOwner);
                ViewData["TotalTime"] = _boatOwnerService.TotalTime(boatOwner);

                return View(boatOwner);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }
    }
}