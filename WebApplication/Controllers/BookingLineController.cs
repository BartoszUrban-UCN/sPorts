using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
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

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [PreventMultipleEvents]
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
                        var success = await _bookingLineService.AddTime(id, (int)amount);

                        if (success)
                        {
                            message = "Time was added to your booking line.";
                            await _bookingLineService.Save();
                        }
                        else
                        {
                            message = "Someone else has booked the spot for that time period. Can NOT add specified time.";
                        }
                    }

                    return RedirectToAction("Details", new { id = id, message = message });
                }
                catch (BusinessException ex)
                {
                    return Content(ex.ToString());
                }
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
