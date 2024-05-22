using ChopSuey.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;

using ChopSuey.Areas.Identity.Data;
using ChopSuey.Contracts;
using ChopSuey.Services;

namespace ChopSuey
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<DbWebChopSuey>();
            services.AddAuthentication();
            services.AddAuthorization();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<AllUserPdfGenerator>();
            services.AddScoped<UserPdfGenareator>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager)
         
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

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
               
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=index}/{id?}");
            });


            app.UseStatusCodePagesWithReExecute("/Error/{0}");


            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=index}/{id?}");
            });

            InitIdentity(userManager, roleManager).Wait();
        }

        private async Task InitIdentity(UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser admin = await userManager.FindByEmailAsync("payam@gmail.com");
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "payam@gmail.com",
                    Email = "payam@gmail.com",
                    EmailConfirmed = true,
                    firstName = "پیام",
                    lastName = "غزنوی",
                    PhoneNumber = "09364154728",
                    age = 36,
                    city = "شیراز"
                };

                var status = await userManager.CreateAsync(admin, "Pp=-123456");
            }
            

            if (await roleManager.RoleExistsAsync("ادمین") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("ادمین"));
            }

            if (await roleManager.RoleExistsAsync("مشتری") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("مشتری"));
            }


            if (await userManager.IsInRoleAsync(admin, "ادمین") == false)
            {
                await userManager.AddToRoleAsync(admin, "ادمین");
            }

            if (await userManager.IsInRoleAsync(admin, "مشتری") == false)
            {
                await userManager.AddToRoleAsync(admin, "مشتری");
            }


        }
    }
}


