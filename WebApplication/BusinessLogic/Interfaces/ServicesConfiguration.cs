using Microsoft.Extensions.DependencyInjection;

namespace WebApplication.BusinessLogic
{
    public static class ServicesConfiguration
    {
        /// <summary>
        /// Extension method for adding all services within the Business layer to the Dependency
        /// Injection framework. Should get called in Startup.cs
        /// </summary>
        /// <param name="services"></param>
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingLineService, BookingLineService>();
            services.AddScoped<IBookingFormService, BookingFormService>();
            services.AddScoped<IBookingConfirmationService, BookingConfirmationService>();

            services.AddScoped<IBoatOwnerService, BoatOwnerService>();
            services.AddScoped<IMarinaOwnerService, MarinaOwnerService>();

            services.AddScoped<IMarinaService, MarinaService>();
            services.AddScoped<IBoatService, BoatService>();

            services.AddScoped<ISpotService, SpotService>();
            services.AddScoped<ILocationService, LocationService>();

            services.AddScoped<ILoginService, LoginService>();
        }
    }
}
