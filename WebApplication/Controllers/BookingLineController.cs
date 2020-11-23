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
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddTime(int? id, [Bind("amount")] int amount)
        {
            try
            {
                await _bookingLineService.AddTime(id, amount);
                return Content("Added");
            }
            catch (BusinessException ex)
            {
                return Content(ex.ToString());
            }
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