using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using backend.DataAccess.Entities.Identity;

using backend.DataAccess;

namespace backend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private static readonly string developerConnectionString = "Server=tcp:dunakanyarhouse-prod-db.database.windows.net,1433;Initial Catalog=dunakanyarhouse-prod-db;Persist Security Info=False;User ID=caribou;Password=R239Aq///;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //private static readonly string developerConnectionString = "Server=localhost;Database=JenniferEstate;User Id=SA;Password=valami888__;";
        private static readonly string clientSideHostDirectoryName = "frontend";
        public static DbContextOptionsBuilder ctxOptionsBuilder => new DbContextOptionsBuilder()
                                                                .UseSqlServer(developerConnectionString, null);
        public static UserManager<User> UserManager;
        public static SignInManager<User> SignInManager;
        public static RoleManager<UserRole> RoleManager;

        public static class Email { 
            public static string Host = "smtp.gmail.com";
            public static int Port = 587;
        }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                
            });

            // Entity Framework élesítése az alkalmazáson.
            //
            services.AddDbContext<JenniferEstateContext>(options => options.UseSqlServer(developerConnectionString, null));


            // Identity élesítése az alkalmazáson.
            //
            var identity = services.AddIdentity<User, UserRole>()
                                   .AddEntityFrameworkStores<JenniferEstateContext>()
                                   .AddDefaultTokenProviders();

            UserManager = services.BuildServiceProvider()
                                  .GetService(typeof(UserManager<User>)) as UserManager<User>;

            SignInManager = services.BuildServiceProvider()
                                    .GetService(typeof(SignInManager<User>)) as SignInManager<User>;

            RoleManager = services.BuildServiceProvider()
                                  .GetService(typeof(RoleManager<UserRole>)) as RoleManager<UserRole>;

            // Identity konfiguráció.
            //
            services.Configure<IdentityOptions>(options => {

                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.";
                    options.User.RequireUniqueEmail = true;
                    
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;

                    options.Lockout.MaxFailedAccessAttempts = 3;
            });

            // Süti konfiguráció.
            //
            services.Configure<CookieOptions>(options => {

                options.IsEssential = true;
                var aging = new System.TimeSpan(0, 15, 0);
                options.MaxAge = aging;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseCookiePolicy();

            // Hitelesítés kényszerítése.
            //
            app.UseAuthentication();

            // URL konfigurációk.
            //
            app.UseMvc(configureRoutes => configureRoutes
                            .MapRoute(
                                "default",
                                "/"
                            )


                            .MapRoute(
                                "estate",
                                "{controller=Estate}/{action=GetEstates}"
                            )
                            .MapRoute(
                                "estate-detailed",
                                "{controller=Estate}/{estateId?}/{action=Detail}"
                            )
                            .MapRoute(
                                "estate-modify",
                                "{controller=Estate}/{estateId?}/{action=Modify}"
                            )
                            .MapRoute(
                                "estate-delete",
                                "{controller=Estate}/{estateId?}/{action=Delete}"
                            )

                            .MapRoute(
                                "employee",
                                "{controller=Employee}/{action=List}"
                            )
                            .MapRoute(
                                "employee-detailed",
                                "{controller=Employee}/{employeeId?}/{action=Bio}"
                            )
                        );
        }
    }
}
