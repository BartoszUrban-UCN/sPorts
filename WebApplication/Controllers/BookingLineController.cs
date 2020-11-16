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
        private readonly IBookingService _bookingService;

        public BookingLineController(SportsContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
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
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Details(int id, int amount = 10)
        {
            var bookingLine = await _context.BookingLines
                .FirstOrDefaultAsync(b => b.BookingLineId == id);

            if (bookingLine == null)
            {
                return NotFound();
            }

            try
            {
                _bookingService.AddTimeToBookingLine(bookingLine, amount);

                return View(bookingLine);

            }
            catch (BusinessException)
            {
                return Content("Something bad happened.🤠");
            }
        }
    }
}