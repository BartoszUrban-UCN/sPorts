using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingLineController : ControllerBase
    {
        private readonly IBookingLineService _bookingLineService;

        public BookingLineController(IBookingLineService bookingLineService)
        {
            _bookingLineService = bookingLineService;
        }

        [HttpPut]
        public async Task<ActionResult<bool>> CancelBookingLine(int id)
        {
            await _bookingLineService.CancelBookingLine(id);
            return Ok();
        }
    }
}
