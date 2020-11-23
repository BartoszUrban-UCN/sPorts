﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingService.GetAll();
            return View(bookings);
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingService.GetSingle(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            //var boats = await _bookingService.GetBoatsByBoatOwner(int boatOwnerId);
            //ViewData["BoatId"] = new SelectList(boats, "BoatId", "BoatId");
            return View();
        }

        // POST: Booking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,BookingReferenceNo,TotalPrice,PaymentStatus,BoatId")] Booking booking, Dictionary<DateTime[], Spot> marinaSpotStayDates)
        {
            if (ModelState.IsValid)
            {
                booking.BookingLines = _bookingService.CreateBookingLines(marinaSpotStayDates);
                await _bookingService.Create(booking);
                return RedirectToAction(nameof(Index));
            }

            //var boats = await _bookingService.GetBoatsByBoatOwner(int boatOwnerId);
            //ViewData["BoatId"] = new SelectList(boats, "BoatId", "BoatId", booking.BoatId);
            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _bookingService.GetSingle(id);
            if (booking == null)
            {
                return NotFound();
            }

            //var boats = await _bookingService.GetBoatsByBoatOwner(int boatOwnerId);
            //ViewData["BoatId"] = new SelectList(boats, "BoatId", "BoatId", booking.BoatId);;
            return View(booking);
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingReferenceNo,TotalPrice,PaymentStatus,BoatId")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _bookingService.Update(booking);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _bookingService.Exists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //var boats = await _bookingService.GetBoatsByBoatOwner(int boatOwnerId);
            //ViewData["BoatId"] = new SelectList(boats, "BoatId", "BoatId", booking.BoatId);
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _bookingService.GetSingle(id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookingService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetBookingLines(int? id)
        {
            try
            {
                var bookingLines = await _bookingService.GetBookingLines(id);
                return View("~/Views/BookingLine/Index.cshtml", bookingLines);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        public IActionResult CancelBookingLine(int? id)
        {
            return RedirectToAction("Cancel", "BookingLine", new { id = id });
        }

        public async Task<IActionResult> Cancel(int? id)
        {
            try
            {
                await _bookingService.CancelBooking(id);
                return Content("Canceled!");
            }
            catch(BusinessException ex)
            {
                return Content(ex.ToString());
            }
        }

        // Random method
        // GET: Booking/CreateBookingMapMingle
        public async Task<IActionResult> CreateBookingMapMingle()
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.MaxValue;

            ViewData["AvailableMarinas"] = _bookingService.GetAllAvailableSpotsCount(new List<int>() { 1, 2, 3, 4 }, 1, startDate, endDate);
            ViewData["Marinas"] = await _bookingService.GetAll();
            return View();
        }

        public IActionResult BookingLineDetails(int? id)
        {
            return RedirectToAction("Details", "BookingLine", new {id = id});
        }
    }
}
