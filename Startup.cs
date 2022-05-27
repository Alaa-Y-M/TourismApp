using GradProj.Models;
using GradProj.Models.Abstract;
using GradProj.Models.Concrete;
using GradProj.Models.SiteModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReflectionIT.Mvc.Paging;
using System;

namespace GradProj
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            
            #region Services_Repositories
            services.AddScoped<IToureRepository<Place>, PlaceRepository>();
            services.AddScoped<IToureRepository<Hotel>, HotelRepository>();
            services.AddScoped<IToureRepository<Room>, RoomRepository>();
            services.AddScoped<IToureRepository<Festival>, FestivalRepository>();
            services.AddScoped<IToureRepository<Restaurant>, RestRepository>();
            services.AddScoped<IToureRepository<City>, CityRepository>();
            services.AddScoped<IToureRepository<Booking>, BookingRepository>();
            services.AddScoped<IToureRepository<Customer>, CustomerRepository>();
            services.AddScoped<IToureRepository<Post>, PostRepository>();
            #endregion
            #region Services_Autentication
            services.Configure<CookiePolicyOptions>(x =>
            {
                x.CheckConsentNeeded = context => true;
                x.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(c =>
            {
                c.Cookie.HttpOnly = true;
                c.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                c.LoginPath= "/Account/Login";
                c.LogoutPath = "/Account/Logout";
                c.SlidingExpiration = true;
            });
            #endregion
            #region Other Services
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationContext>(option => option.UseSqlServer(Configuration.GetConnectionString("GradDB")));
            services.AddDbContext<SiteContext>(o => o.UseSqlServer(Configuration.GetConnectionString("GradDB")));
            services.AddIdentity<ApplcationUser, ApplicationRole>(o=> {
                o.Password.RequireDigit = true;
                o.Password.RequiredLength = 6;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequiredUniqueChars = 1;
                o.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            services.AddPaging(option=>option.ViewName= "Bootstrap4");
            #endregion
        }

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
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
