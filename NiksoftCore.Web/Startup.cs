using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NiksoftCore.DataModel;
using System;
using System.Text;

namespace NiksoftCore.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SystemBase")));

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<Role>()
                .AddEntityFrameworkStores<AuthDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddScoped<ISystemBaseService, SystemBaseService>();

            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
            opt =>
            {
                //configure your other properties
                opt.LoginPath = "/Auth/Account/Login";
                opt.AccessDeniedPath = "/Auth/Account/Login";
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = false;
            });

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenOptions:Key"]));

            services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Auth/Account/Login";
                options.LogoutPath = "/Auth/Account/Logout";
                options.AccessDeniedPath = "/Auth/Account/Login";
                options.SlidingExpiration = true;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = "niksoftgroup.ir",
                    ValidAudience = "niksoftgroup.ir",
                    IssuerSigningKey = symmetricSecurityKey
                };
            });

            //services.AddAuthorization(options => options.AddPolicy("AdminAccess", policy => {
            //    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
            //    policy.RequireAuthenticatedUser();
            //    policy.RequireRole("NikAdmin");
            //}));

            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                //endpoints.MapAreaControllerRoute(
                //    name: "Business",
                //    areaName: "Business",
                //    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                //).RequireHost("localhost:49602", "shop.fbiic.ir");

                endpoints.MapRazorPages();

            });
        }

        //private static IServiceProvider DepInjectionProvider(IServiceCollection services) {
        //    var builder = new ContainerBuilder();
        //    builder.Populate(services);
        //    builder.RegisterType<SystemBaseService>().As<ISystemBaseService>();
        //    IContainer applicationContainer = builder.Build();
        //    return new AutofacServiceProvider(applicationContainer);
        //}

    }
}