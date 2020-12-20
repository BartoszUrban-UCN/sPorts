using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.Authorization;
using WebApplication.BusinessLogic;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookingLineController : Controller
    {
        private readonly IBookingLineService _bookingLineService;
        private readonly IBookingFormService _bookingFormService;
        private readonly IBoatService _boatService;

        public BookingLineController(IBookingLineService bookingLineService, IBookingFormService bookingFormService, IBoatService boatService)
        {
            _bookingLineService = bookingLineService;
            _bookingFormService = bookingFormService;
            _boatService = boatService;
        }

        public async Task<IActionResult> Details(int? id, string message = "")
        {
            var isAuthorized =
               User.IsInRole(RoleName.Administrator) ||
               User.IsInRole(RoleName.Manager) || User.IsInRole(RoleName.BoatOwner);

            if (isAuthorized)
            {
                var bookingLine = await _bookingLineService.GetSingle(id);
                ViewData["message"] = message;

                return View(bookingLine);
            }

            return Forbid();
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var isAuthorized =
                User.IsInRole(RoleName.Administrator) ||
                User.IsInRole(RoleName.Manager) || User.IsInRole(RoleName.BoatOwner);

            if (isAuthorized)
            {
                try
                {
                    await _bookingLineService.CancelBookingLine(id);
                    return RedirectToAction("Index");
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
