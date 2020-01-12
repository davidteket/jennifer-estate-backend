using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using DunakanyarHouseIngatlan.Services;
using DunakanyarHouseIngatlan.DataAccess;
using DunakanyarHouseIngatlan.DataAccess.Entities.Identity;

namespace DunakanyarHouseIngatlan
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private static string _developerConnectionString;
        public static string DeveloperConnectionString {
            get {
                return _developerConnectionString;
            }
        }

        private static string _developerUser;
        public static string DeveloperUser {
            get {
                return _developerUser;
            }
        }

        private static string _developerPassword;
        public static string DeveloperPassword {
            get {
                return _developerPassword;
            }
        }

        private static string _developerRole;
        public static string DeveloperRole {
            get {
                return _developerRole;
            }
        }

        private string _developerFirstName;
        public string DeveloperFirstName {
            get {
                return _developerFirstName;
            }
        }

        private string _developerLastName;
        public string DeveloperLastName {
            get {
                return _developerLastName;
            }
        }

        private string _developerEmail;
        public string DeveloperEmail {
            get {
                return _developerEmail;
            }
        }
        
        private Serializer _serializer;
        public static DbContextOptionsBuilder ctxOptionsBuilder => new DbContextOptionsBuilder()
                                                                .UseSqlServer(_developerConnectionString, null);
        public static UserManager<User> UserManager;
        public static SignInManager<User> SignInManager;
        public static RoleManager<UserRole> RoleManager;
        

        // Alkalmazás konfiguráció beállítások.
        //
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _serializer = new Serializer();
            _developerConnectionString = _serializer.GetApplication("DeveloperConnectionString");
            _developerUser = _serializer.GetApplication("User");
            _developerPassword = _serializer.GetApplication("Password");
            _developerRole = _serializer.GetApplication("Role");
            _developerFirstName = _serializer.GetApplication("FirstName");
            _developerLastName = _serializer.GetApplication("LastName");
            _developerEmail = _serializer.GetApplication("Email");

        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                
            });

            // Dependency Injection.
            //
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<SignInManager<User>, SignInManager<User>>();

            // Entity Framework.
            //
            services.AddDbContext<JenniferEstateContext>(options => options.UseSqlServer(_developerConnectionString, null));


            // Identity.
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

            // Naplózás.
            //
            services.AddLogging(logging => 
            {
                // TODO
            });

            // Identity konfiguráció.
            //
            services.Configure<IdentityOptions>(options => {

                    options.User.AllowedUserNameCharacters = _serializer.GetDefaults("TmpPassRegularCharPool");
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

            var checkRootUserExsistenceTask = Startup.UserManager.FindByNameAsync(_developerUser);
            checkRootUserExsistenceTask.Wait();

            if (checkRootUserExsistenceTask.IsCompletedSuccessfully && checkRootUserExsistenceTask.Result != null) {
                System.Console.WriteLine(_serializer.GetServerLogMessage("RootUserExists"));
            }
            else {

                var role = new UserRole();
                role.Name = _developerRole;
                var createRootRoleTask = Startup.RoleManager.CreateAsync(role);
                createRootRoleTask.Wait();

                if (createRootRoleTask.IsCompletedSuccessfully)
                    System.Console.WriteLine(_serializer.GetServerLogMessage("RootRoleCreated"));
                else
                    System.Console.WriteLine(_serializer.GetServerLogMessage("RootRoleError"));

                var user = new User();
                user.UserName = _developerUser;
                user.FirstName = _developerFirstName;
                user.LastName = _developerLastName;
                user.Email = _developerEmail;
                
                var createRootUserTask = Startup.UserManager.CreateAsync(user, _developerPassword);
                createRootUserTask.Wait();

                if (createRootUserTask.IsCompletedSuccessfully && createRootUserTask.Result != null)
                    System.Console.WriteLine(_serializer.GetServerLogMessage("RootUserCreated"));
                else 
                    System.Console.WriteLine(_serializer.GetServerLogMessage("RootUserError"));

                var getRootUserTask = Startup.UserManager.FindByNameAsync(_developerUser);
                getRootUserTask.Wait();

                if (getRootUserTask.IsCompletedSuccessfully && getRootUserTask.Result != null)
                {
                    var addtoRoleTask = Startup.UserManager.AddToRoleAsync(getRootUserTask.Result, _developerRole);
                    addtoRoleTask.Wait();
                    if (addtoRoleTask.IsCompletedSuccessfully)
                        System.Console.WriteLine(_serializer.GetServerLogMessage("RootUserToRootRole"));
                    else
                        System.Console.WriteLine(_serializer.GetServerLogMessage("RootUserToRootRoleError"));
                }
            }


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

            app.UseHttpsRedirection();

            // URL konfigurációk.
            //
            app.UseMvc(configureRoutes => configureRoutes
                            .MapRoute(
                                "default",
                                "/home.html"
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
