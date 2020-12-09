using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebApplication.Authorization;
using WebApplication.BusinessLogic;
using WebApplication.Data;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookingLineController : Controller
    {
        private readonly SportsContext _context;
        private readonly IBookingLineService _bookingLineService;
        public BookingLineController(SportsContext context, IBookingLineService bookingLineService)
        {
            _context = context;
            _bookingLineService = bookingLineService;
        }
        public async Task<IActionResult> Details(int? id, string message = "")
        {
            var bookingLine = await _bookingLineService.GetSingle(id);
            ViewData["message"] = message;

            return View(bookingLine);
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
                            message = "Someone else has booked the spot after you. Can NOT add more time.";
                    }

                    return RedirectToAction("Details", new { id = id, message = message });
                }
                catch (BusinessException ex)
                {
                    //return Content(ex.ToString());
                }
            }

            return Forbid();
        }

        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _bookingLineService.CancelBookingLine(id);
                return Content("Cancel");
            }
            catch (BusinessException ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}