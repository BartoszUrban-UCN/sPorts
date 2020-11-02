using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Business_Logic;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class SpotController : Controller
    {
        private readonly SportsContext _context;

        public SpotController(SportsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Spot> spots = await _context.Spots.ToListAsync();
            return View(spots);
        }
    }
}

