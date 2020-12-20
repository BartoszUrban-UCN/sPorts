using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "Manager,BoatOwner")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BoatOwnerController : Controller
    {
        private readonly IBoatOwnerService _boatOwnerService;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserService _userService;

        public BoatOwnerController(IBoatOwnerService boatOwnerService, IAuthorizationService authorizationService, UserService userService)
        {
            _boatOwnerService = boatOwnerService;
            _authorizationService = authorizationService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _boatOwnerService.GetAll();
            return View(result);
        }

        [Authorize(Roles = "BoatOwner")]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userService.GetUserAsync(User);
            try
            {
                var currentBoatOwner = _userService.GetBoatOwnerFromPerson(user);
                var bookings = await _boatOwnerService.GetBookings(currentBoatOwner.BoatOwnerId);
                return View("~/Views/Booking/Index.cshtml", bookings);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
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
