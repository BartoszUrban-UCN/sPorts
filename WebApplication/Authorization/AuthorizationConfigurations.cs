using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Authorization.Shared;
using WebApplication.Authorization.MarinaOwner;

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

            // Managers are authorized to do anything currently
            services.AddSingleton<IAuthorizationHandler, ManagerAuthorizationHandler<object>>();
        }
    }
}
