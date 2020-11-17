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
    public class BoatOwnerController : Controller
    {
        private readonly SportsContext _context;
        private readonly IBoatOwnerService _boatOwnerService;
        public BoatOwnerController(SportsContext context, IBoatOwnerService service)
        {
            _context = context;
            _boatOwnerService = service;
        }

        public async Task<IActionResult> Index()
        {
            var boatOwners = _context.BoatOwners.Include(b => b.Person);
            return View(await boatOwners.ToListAsync());
        }

        public async Task<IActionResult> GetBookings(int id)
        {
            try
            {
                var bookings = await _boatOwnerService.Bookings(id);
                return View("~/Views/Booking/Index.cshtml", bookings);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> GetOngoingBookings(int id)
        {
            try
            {
                var ongoingBookings = await _boatOwnerService.OngoingBookings(id);
                return View("~/Views/Booking/Index.cshtml", ongoingBookings);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> GetBoats(int id)
        {
            var boatOwnerWithBoats = await _context.BoatOwners.Include(b => b.Boats)
                                                        .ToListAsync();
            var boatOwner = boatOwnerWithBoats.FirstOrDefault(b => b.BoatOwnerId == id);

            if (boatOwner != null)
            {
                return View("~/Views/Boats/Index.cshtml", boatOwner.Boats);
            }
            return View("Error");
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var boatOwner = await _boatOwnerService.FindBoatOwner(id);
                ViewData["TotalSpent"] = _boatOwnerService.MoneySpent(boatOwner);
                ViewData["TotalTime"] = _boatOwnerService.TotalTime(boatOwner);
                return View(boatOwner);
            }
            catch(BusinessException)
            {
                return View("Error");
            }
        }
    }
}