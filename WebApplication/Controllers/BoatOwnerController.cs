using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class BoatOwnerController : Controller
    {
        private readonly SportsContext _context;

        public BoatOwnerController(SportsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var boatOwners = _context.BoatOwners;
            return View(await boatOwners.ToListAsync());
        }
    }
}