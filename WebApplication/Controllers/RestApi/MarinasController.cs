using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

// Testing
using System.Text.Json;
using System.Text.Json.Serialization;
// Testing

using WebApplication.Models;
using WebApplication.Data;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarinasController : ControllerBase
    {
        // Dependency injection for the EF context + reference
        private readonly SportsContext _context;

        public MarinasController(SportsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Marina> GetMarinas()
        {
            return _context.Marinas;
        }
    }
}