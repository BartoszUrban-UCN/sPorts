using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Interfaces;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class PaymentController : Controller
    {
        private readonly SportsContext _context;

        private readonly IPaymentService _paymentService;

        public PaymentController(SportsContext context,IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        // GET: Payment
        public async Task<IActionResult> Index()
        {
            var result = await _paymentService.GetAll();
            return View(result);
        }

        // GET: Payment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var payment = await _paymentService.GetSingle(id);
                return View(payment);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        // GET: Payment/Create
        public async Task<IActionResult> CreateFromBooking()
        {
            //Session 
            //var booking = HttpContext.Session.Get<Booking>("Booking");
            
            //Mocking abooking untill Session work 
            var booking = _context.Bookings.Find(1);        
            
            var payment= await _paymentService.CreateFromBooking(booking);
            ViewData["BookingId"] = booking.BookingId;
            return View("~/Views/Payment/Create.cshtml", payment);
        }


        // POST: Payment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,IncomingPaymentReference,IncomingPaymentStatus,InvoiceStatus")] Payment payment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _paymentService.Create(payment);
                    return RedirectToAction(nameof(Index));
                }

                var paymentId = ViewData["PaymentId"];
                return View(payment);
            }
            catch (BusinessException)
            { }
            return View("Error");
        }
         
      

        // GET: Payment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var payment = await _paymentService.GetSingle(id);
                var paymentId = ViewData["PaymentId"];
                return View(payment);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        // POST: Payment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,IncomingPaymentReference,IncomingPaymentStatus,InvoiceStatus")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _paymentService.Update(payment);
                    var paymentId = ViewData["PaymentId"];
                    return View(payment);
                }
            }
            catch (BusinessException)
            {
                
            }
            return View("Error");

        }

        // GET: Payment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var payment = await _paymentService.GetSingle(id);
                return View(payment);
            }
            catch (BusinessException)
            { }

            return View("Error");
        }

        // POST: Payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _paymentService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException)
            { }

            return View("Error");
        }

        private async Task<bool> PaymentExists(int id)
        {
            return await _paymentService.Exists(id);
        }

        private async Task<IActionResult> StartPayment(Payment payment)
        {
            try
            {
                await _paymentService.StartPayment(payment);
                return View(payment);
            }
            catch (BusinessException)
            { }

            return View("Error");
        }
    }
}
