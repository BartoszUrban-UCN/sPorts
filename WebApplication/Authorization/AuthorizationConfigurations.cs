using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Authorization.Shared;
using WebApplication.Authorization.MarinaOwner;
using WebApplication.Authorization.BoatOwner;
using Microsoft.AspNetCore.Authorization.Policy;

namespace WebApplication.Authorization
{
    public static class AuthorizationConfiguration
    {
        /// <summary>
        /// Extension method for adding all necessary authorizations services to the Dependency
        /// Injection framework. Should get called in Startup.cs
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationService, DefaultAuthorizationService>();
            services.AddScoped<IAuthorizationHandlerProvider, DefaultAuthorizationHandlerProvider>();
            services.AddScoped<IPolicyEvaluator, PolicyEvaluator>();
        }

        /// <summary>
        /// Extension method for adding all necessary handlers for the application to the Dependency
        /// Injection framework. Should get called in Startup.cs
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuthorizationHandlers(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, BoatOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, MarinaOwnerAuthorizationHandler>();

            // Managers are authorized to do anything currently
            services.AddScoped<IAuthorizationHandler, ManagerAuthorizationHandler<object>>();
        }
    }
}
