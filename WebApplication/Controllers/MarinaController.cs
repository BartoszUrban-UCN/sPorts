using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var marinas = _context.Marinas.ToListAsync();

            return View("_ListLayout", await marinas);
        }

        [HttpGet]
        public async Task<IActionResult> GetMarinas()
        {
            ViewData["ViewName"] = "Marina";

            var marinas = _context.Marinas.ToListAsync();

            return View("_ListLayout", await marinas);
        }

        [HttpGet]
        [Route("marina/{id}")]
        public async Task<IActionResult> GetMarina(int id)
        {
            ViewData["ViewName"] = "Marina";

            var marina = await _context.Marinas.FindAsync(id);

            if (marina != null)
            {
                var marinaList = new List<Marina>();
                marinaList.Add(marina);
                return View("_ListLayout", marinaList);
            }
            return View("Error");
        }


        [HttpGet]
        [Route("{id}/spots")]
        public async Task<IActionResult> GetMarinaSpots(int id)
        {
            var marinaWithSpots = await _context.Marinas.Include(s => s.Spots).ToListAsync();
            var marina = marinaWithSpots.Find(m => m.MarinaId == id);

            ViewData["ViewName"] = "~/Views/Spot/Spot.cshtml";
            if (marina != null)
            {
                var spots = marina.Spots;
                return View("_ListLayout", spots);
            }
            return View("Error");
        }
    }
}
