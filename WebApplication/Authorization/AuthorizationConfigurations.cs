using Microsoft.Extensions.DependencyInjection;
using WebApplication.BusinessLogic.Interfaces;
using WebApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Authorization
{
    public static class AuthorizationConfiguration
    {
        /// <summary>
        /// Extension method for adding all services within the Business layer to the Dependency
        /// Injection framework. Should get called in Startup.cs
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, MarinaOwnerAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, MarinaManagerAuthorizationHandler>();
        }
    }
}
