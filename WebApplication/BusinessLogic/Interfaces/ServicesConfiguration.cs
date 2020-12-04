using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WebApplication.BusinessLogic.Interfaces;
using WebApplication.Models;

namespace WebApplication.BusinessLogic
{
    public static class ServicesConfiguration
    {
        /// <summary>
        /// Extension method for adding all services within the Business layer
        /// to the Dependency Injection framework. Should get called in Startup.cs
        /// </summary>
        /// <param name="services"></param>
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingLineService, BookingLineService>();
            services.AddScoped<IBookingFormService, BookingFormService>();

            services.AddScoped<IBoatOwnerService, BoatOwnerService>();
            services.AddScoped<IMarinaOwnerService, MarinaOwnerService>();

            services.AddScoped<IBoatService, BoatService>();

            services.AddScoped<IMarinaService, MarinaService>();
            services.AddScoped<ISpotService, SpotService>();
            services.AddScoped<ILocationService, LocationService>();

            services.AddScoped<IPDFService<Booking>, BookingPDFService>();

            services.AddScoped<IPaymentService, PaymentService>();
        }
    }
}
