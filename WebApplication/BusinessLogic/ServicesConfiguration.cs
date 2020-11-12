using Microsoft.Extensions.DependencyInjection;
using WebApplication.Business_Logic;

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
            services.AddScoped<ILoginService, LoginService>();
        }
    }
}
