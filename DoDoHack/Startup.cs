using DoDoHack.Data;
using DoDoHack.Extensions;
using DoDoHack.SignalHubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DoDoHack
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string dbConnectionStr = Configuration.GetConnectionString("DbConnection");
            services.AddDbContext<DodoBase>(options => options.UseSqlServer(dbConnectionStr));

            string privateKey = Configuration.GetValue<string>("PrivateKey");
            services.AddHashService(privateKey);

            services.AddStartupDbEntities(Configuration);

            services.AddOrderDistributionService();

            services.AddEmailSender();

            services.AddSupportService();

            services.AddFileService();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = "/Login";
                    options.AccessDeniedPath = "/Denied";
                });

            services.AddSignalR();
            services.AddSignalUserIdProvider();

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TrackHub>("/couriertrack");
                endpoints.MapHub<LineChatHub>("/linechat");
                endpoints.MapHub<SupportHub>("/support");
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapHub<CourierChatHub>("/courierchat");
                endpoints.MapRazorPages();
            });
        }
    }
}
