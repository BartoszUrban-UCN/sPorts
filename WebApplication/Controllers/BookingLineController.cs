using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic;

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
            var bookingLine = await _context.BookingLines
                .FirstOrDefaultAsync(b => b.BookingLineId == id);
            if (bookingLine == null)
            {
                return NotFound();
            }

            return View(bookingLine);
        }

        [HttpPost]
        [Route("bookingline/{id}/addtime", Name="addtime")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddTime(int id, [Bind("amount")]int amount)
        {
            var success = await _bookingLineService.AddTime(id, amount);

            if (success)
            {
                return Content("Added");
            }
            return Content("Not Added");
        }

        [Route("bookingline/{id}/cancel", Name="cancelbline")]
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