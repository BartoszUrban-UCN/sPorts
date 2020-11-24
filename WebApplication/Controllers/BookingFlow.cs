using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class BookingFlow : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IBoatService _boatService;
        private readonly IMarinaService _marinaService;

        public BookingFlow(IBookingService bookingService, IBoatService boatService, IMarinaService marinaService)
        {
            _bookingService = bookingService;
            _boatService = boatService;
            _marinaService = marinaService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Boat boat)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    boat = await _boatService.GetSingleByName(boat.Name);
                    var booking = new Booking { BoatId = boat.BoatId, Boat = boat };
                    var bookingLine = new BookingLine { Booking = booking };
                    return RedirectToAction("ChooseMarina", bookingLine);//return View("ChooseMarina", bookingLine); // CHANGED: SHOULD BE CHOOSE DATES
                }
                catch (BusinessException)
                {
                }
            }

            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "Name", "Name");
            return View();
        }

        public async Task<IActionResult> ChoseDates(BookingLine bookingLine)
        {
            return View(bookingLine);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChoseDates(BookingLine bookingLine)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            return View("ChoseMarina", bookingLine);
        //        }
        //        catch (BusinessException)
        //        {
        //        }
        //    }
        //    return View(bookingLine);
        //}

        // GET: BookingFlow
        public async Task<IActionResult> ChooseMarina(BookingLine bookingLine)
        {
            //StringValues result = Request.Form["bookButton"];

            if (true)
            {
                try
                {
                    Console.WriteLine("success");
                }
                catch (BusinessException)
                {

                }
            }

            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.MaxValue;

            ViewData["AvailableMarinas"] = _bookingService.GetAllAvailableSpotsCount(new List<int>() { 1, 2, 3, 4 }, 1, startDate, endDate);
            ViewData["AllMarinas"] = await _marinaService.GetAll();
            return View(bookingLine);
        }

        // Random method
        // GET: Booking/CreateBookingMapTry
        //public async Task<IActionResult> CreateBookingMapTry()
        //{
        //    DateTime startDate = DateTime.Now;
        //    DateTime endDate = DateTime.MaxValue;

        //    ViewData["AvailableMarinas"] = _bookingService.GetAllAvailableSpotsCount(new List<int>() { 1, 2, 3, 4 }, 1, startDate, endDate);
        //    ViewData["Marinas"] = await _marinaService.GetAll();
        //    return View();
        //}
    }
}
