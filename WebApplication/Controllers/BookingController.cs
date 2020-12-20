using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IBoatService _boatService;

        public BookingController(IBookingService bookingService, IBoatService boatService)
        {
            _bookingService = bookingService;
            _boatService = boatService;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingService.GetAll();
            return View(bookings);
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingService.GetSingle(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        public IActionResult BookingLineDetails(int? id)
        {
            return RedirectToAction("Details", "BookingLine", new { id = id });
        }
    }
}
