using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using WebApplication.Data;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "listing",
                    defaults: new { action = "Index" },
                    pattern: "{controller}/{id?}/{action}"
                );
            });

            //Swagger configuration
            app.UseSwagger();

            app.UseSwaggerUI(swagger =>
            {
                swagger.SwaggerEndpoint("v1/swagger.json", "sPorts API v1");
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            //Configure DbContext, connect to the LocalDB
            string dbString = "LocalDb";
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                dbString = "DragosDb"; // @Dragos there you go (i hope it works)
            }

            services.AddDbContext<SportsContext>(options => options.UseSqlServer(Configuration.GetConnectionString(dbString)));

            //Swagger service
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "sPorts API",
                        Description = "The API for the sPorts distributed system",
                        //TermsOfService = new Uri(""),
                        Contact = new OpenApiContact
                        {
                            Name = "Group 1",
                            Email = string.Empty,
                            //Url = new Uri(""),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Use under LICX",
                            //Url = new Uri(""),
                        }
                    });

                //Point swagger to the generated xml file
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);
            });
        }
    }
}
