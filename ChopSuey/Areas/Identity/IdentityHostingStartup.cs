using System;
using ChopSuey.Areas.Identity.Data;
using ChopSuey.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ChopSuey.Areas.Identity.IdentityHostingStartup))]
namespace ChopSuey.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<DbWebChopSuey>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DbWebChopSueyConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                  .AddRoles<IdentityRole>().AddEntityFrameworkStores<DbWebChopSuey>();
                services.AddAuthorization(x =>
                {
                    x.AddPolicy("Adminpolicy", p => p.RequireRole("ادمین"));
                   

                });
            });
        }
    }
}
