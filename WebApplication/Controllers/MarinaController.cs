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
    public class MarinaController : Controller
    {
        private readonly SportsContext _context;

        public MarinaController(SportsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Marina> marinas = await _context.Marinas.ToListAsync();
            return View(marinas);
        }

        [Route("marina/{id}")]
        public async Task<IActionResult> Marina(int? id)
        {
            var marina = _context.Marinas.FindAsync(id);
            return View(await marina);
        }

        public async Task<IActionResult> Spots(int id)
        {
            var marinaWithSpots = await _context.Marinas.Include(s => s.Spots).ToListAsync();
            var marina = marinaWithSpots.Find(marina => marina.MarinaId == id);
            var spots = marina.Spots;

            if (marina != null)
            {
                return View("~/Views/Spot/Index.cshtml", spots);            
            }
            return View("Error");
        }
    }
}
