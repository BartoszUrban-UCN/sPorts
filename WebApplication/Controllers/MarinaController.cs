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
            ViewData["ViewName"] = "Marina";
            List<Marina> marinas = await _context.Marinas.ToListAsync();
            return View("_ListLayout", marinas);
        }

        [Route("marina/{id}")]
        public async Task<IActionResult> Marina(int? id)
        {
            ViewData["ViewName"] = "Marina";
            var marina = _context.Marinas.FindAsync(id);
            List<Marina> marinaList = new List<Marina>();
            marinaList.Add(await marina);
            return View("_ListLayout", marinaList);
        }

        public async Task<IActionResult> Spots(int id)
        {
            var marinaWithSpots = await _context.Marinas.Include(s => s.Spots).ToListAsync();
            var marina = marinaWithSpots.Find(marina => marina.MarinaId == id);
            var spots = marina.Spots;

            ViewData["ViewName"] = "~/Views/Spot/Spot.cshtml";
            if (marina != null)
            {
                return View("_ListLayout", spots);            
            }
            return View("Error");
        }
    }
}
