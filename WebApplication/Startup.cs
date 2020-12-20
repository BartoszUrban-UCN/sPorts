using ElectronNET.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebApplication.Authorization;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var supportedCultures = new string[] { "en-US", "en-GB", "da-DK", "pl-PL", "ro-RO" };
            app.UseRequestLocalization(options =>
                        options
                        .AddSupportedCultures(supportedCultures)
                        .AddSupportedUICultures(supportedCultures)
                        .SetDefaultCulture("en-US")
                );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                //app.UseSPortsExceptionHandler();
                app.UseExceptionHandler("/Home/Error");
                app.UseSPortsExceptionHandler();
                // The default HSTS value is 30 days. You may want to change
                // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Session configuration
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

            //Swagger configuration
            app.UseSwagger();

            app.UseSwaggerUI(swagger =>
            {
                swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "sPorts API v1");
            });

            // Electron Setup
            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());

            // Stripe payment gateway Setup
            Stripe.StripeConfiguration.ApiKey = "sk_test_51HwQzXFX3jJVK0RybqLl9m5xVOzUVLs5g5tAwyNM4IiNGtsc0ppOTCFiRG5KVYBQgchgtivwKhemjQz5ohKSVhio00TpqPXKyi";
        }

        // This method gets called by the runtime. Use this method to add
        // services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add sessions
            services.AddDistributedMemoryCache();

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.CheckConsentNeeded = context => true; // consent required
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });

            // MVC and Razor pages support
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddRazorPages();

            // EF Core Context and Database
            string dbString = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "LocalDb" : "DragosDb";

            services.AddDbContext<SportsContext>(options => options.UseSqlServer(Configuration.GetConnectionString(dbString)));

            // Add and Configure Identity
            services.AddIdentity<Person, Role>()
                .AddEntityFrameworkStores<SportsContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddUserManager<UserService>();

            // More convenient password options at the beginning since we are developing
            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;

                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            // Adds JWT Authentication
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => Configuration.Bind("JwtSettings", options));

            // Adds authorization
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                   .RequireAuthenticatedUser()
                   .Build();
            });

            // Adds all authorization services
            services.AddAuthorizationServices();
            // Adds all authorization handlers
            services.AddAuthorizationHandlers();

            // Swagger service
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "sPorts API",
                        Description = "The API for the sPorts distributed system",
                        Contact = new OpenApiContact
                        {
                            Name = "Group 1",
                            Email = string.Empty,
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Use under LICX",
                        }
                    });

                // Point swagger to the generated xml file
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);
            });

            // Adds all services in the Business Layer for dependency injection
            services.AddBusinessServices();
        }
    }
}
