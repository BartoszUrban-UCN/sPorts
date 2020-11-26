using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class BookingFlowController : Controller
    {
        private readonly IBookingFormService _bookingFormService;
        private readonly IBookingService _bookingService;
        private readonly IBoatService _boatService;
        private readonly IMarinaService _marinaService;

        public BookingFlowController(IBookingFormService bookingFormService, IBookingService bookingService, IBoatService boatService, IMarinaService marinaService)
        {
            _bookingFormService = bookingFormService;
            _bookingService = bookingService;
            _boatService = boatService;
            _marinaService = marinaService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "BoatId", "Name");

            var booking = await _bookingFormService.CreateBooking();
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost(Booking booking)
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "BoatId", "Name");
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMarinaMap(string boat, string start, string end)
        {
            var boatId = int.Parse(boat);
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            //var availableSpotsPerMarina = await _bookingFormService.GetAvailableSpotsPerMarina(await _marinaService.GetAll(), boatName, startDate, endDate);
            //var jsonString = HelperMethods.Serialize(availableSpotsPerMarina);

            var jsonString = HelperMethods.Serialize(await _marinaService.GetAll());

            return new JsonResult(jsonString);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpotMap(string boat, string start, string end, string marina)
        {
            var boatId = int.Parse(boat);
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);
            var marinaId = int.Parse(marina);

            //var availableSpotsPerMarina = await _bookingFormService.GetAvailableSpotsPerMarina(await _marinaService.GetAll(), boatName, startDate, endDate);
            //var jsonString = HelperMethods.Serialize(availableSpotsPerMarina);

            var jsonString = HelperMethods.Serialize(await _bookingFormService.GetAvailableSpots(marinaId, boatId, startDate, endDate));

            return new JsonResult(jsonString);
        }

        public async Task<IActionResult> ShoppingCart()
        {
            // explicit load needed??
            var booking = HttpContext.Session.Get<Booking>("Booking");
            var validBooking = _bookingService.ValidateShoppingCart(booking);

            bool hasChanged = false;
            if (!validBooking.Equals(booking))
            {
                hasChanged = true;
            }

            ViewData["BookingChange"] = hasChanged;

            var now = DateTime.Now;
            var then = now.AddDays(1);

            var person = new Person{FirstName="Jonny", Email="jimmyjackson@gmail.com"};
            var person1 = new Person{FirstName="Elton", Email="eltonjohn@gmail.com"};

            var marinaOwner = new MarinaOwner{Person=person};

            var marina = new Marina{MarinaId=1, Name="Me gusta mucho", MarinaOwner=marinaOwner};
            var marina1 = new Marina{MarinaId=2, Name="Aalborg Zoo", MarinaOwner=marinaOwner};
            var marina2 =new Marina{MarinaId=3, Name="Underwater", MarinaOwner=marinaOwner};


            var boatOwner = new BoatOwner{Person=person};

            var boat = new Boat{Name="Mama Destroyer", BoatOwner=boatOwner};


            var spot = new Spot{SpotId=1, Price=12.3d, Marina=marina};
            var spot1 = new Spot{SpotId=2, Price=5.5d, Marina=marina};
            var spot2 = new Spot{SpotId=3, Price=8.9d, Marina=marina};
            var spot3 = new Spot{SpotId=4, Price=3.3d, Marina=marina1};
            var spot4 = new Spot{SpotId=5, Price=8.5d, Marina=marina2};

            ViewData["Discount"] = 10;

            var bookingLine = new BookingLine { StartDate = now, EndDate = then, Spot = spot };
            var bookingLine1 = new BookingLine { StartDate = now, EndDate = then, Spot = spot1 };
            var bookingLine2 = new BookingLine { StartDate = now, EndDate = then, Spot = spot2 };
            var bookingLine3 = new BookingLine { StartDate = now, EndDate = then, Spot = spot3 };
            var bookingLine4 = new BookingLine { StartDate = now, EndDate = then, Spot = spot4 };


            var booking1 = new Booking{BookingReferenceNo=2543, TotalPrice=300, PaymentStatus="Not Paid", Boat=boat, BookingLines=new List<BookingLine>{bookingLine, bookingLine1, bookingLine2, bookingLine3, bookingLine4}};

            var marinaBLineDict = new Dictionary<Marina, IEnumerable<BookingLine>>(); 

            var total = 0.0d;

            foreach(var bLine in booking1.BookingLines)
            {   
                var key = bLine.Spot.Marina;
                var value = bLine;

                if (!marinaBLineDict.ContainsKey(key))
                {
                    marinaBLineDict.Add(key: key, value: new List<BookingLine>{value});
                }
                else
                {
                    ((List<BookingLine>) marinaBLineDict[key]).Add(value);
                }
                total += bLine.DiscountedTotalPrice;
            }

            ViewData["Total"] = total;
            ViewData["MarinaBLineDict"] = marinaBLineDict;

            return View("~/Views/Booking/ShoppingCart.cshtml", booking1);
        }

        public async Task<IActionResult> CartRemoveBookingLine(BookingLine bookingLine)
        {
            var booking = HttpContext.Session.Get<Booking>("Booking");
            var newBooking = _bookingService.CartRemoveBookingLine(booking, bookingLine);

            //HttpContext.Session.Remove("Booking");
            HttpContext.Session.Add<Booking>("Booking", newBooking);

            return RedirectToAction("ShoppingCart");
        }

        public async Task<IActionResult> ClearCart()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
