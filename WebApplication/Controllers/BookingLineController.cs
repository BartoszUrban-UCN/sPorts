using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Details(int id)
        {
            var bookingLine = await _bookingLineService.GetSingle(id);
            if (bookingLine == null)
            {
                return NotFound();
            }

            return View(bookingLine);
        }

        [HttpPost]
        [Route("bookingline/{id}/addtime", Name = "addtime")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddTime(int id, [Bind("amount")] int amount)
        {
            var success = await _bookingLineService.AddTime(id, amount);

            if (success)
            {
                return Content("Added");
            }
            return Content("Not Added");
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _bookingLineService.CancelBookingLine(id);
            if (success)
            {
                return Content("Canceled!");
            }
            return Content("Not Canceled!");
        }
    }
}