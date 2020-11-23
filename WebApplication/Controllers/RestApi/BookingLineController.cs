using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Data;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingLineController : ControllerBase
    {
        private readonly SportsContext _context;
        private readonly IBookingLineService _bookingLineService;

        public BookingLineController(SportsContext context, IBookingLineService bookingLineService)
        {
            _context = context;
            _bookingLineService = bookingLineService;
        }

        [HttpPut]
        public async Task<ActionResult<bool>> CancelBookingLine(int id)
        {
            var success = await _bookingLineService.CancelBookingLine(id);
            return Ok(success);
        }
    }
}