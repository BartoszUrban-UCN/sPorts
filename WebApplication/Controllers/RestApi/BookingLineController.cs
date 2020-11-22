using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic;

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