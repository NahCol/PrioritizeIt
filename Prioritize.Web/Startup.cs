using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Prioritize.Data;
using Prioritize.Models;
using Prioritize.Service;

namespace Prioritize.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<AppConfig>(Configuration.GetSection("Settings"));
            services.AddScoped(c => c.GetService<IOptionsSnapshot<AppConfig>>().Value);

            services.AddTransient<AppConfigService>();
            var sp = services.BuildServiceProvider();

            AppConfig appConfig = sp.GetService<AppConfigService>().AppConfig;

            services.AddScoped((config) => {
                return new SmtpClient
                {
                    Host = appConfig.ServerName,
                    Port = appConfig.ServerPort,
                    EnableSsl = appConfig.EnableSSL
                };
            });
            services.AddTransient<EmailService>();
            
            services.AddTransient<ItemRepository>();
            services.AddTransient<ItemService>();
            services.AddTransient<UserService>();
            services.AddTransient<UserRepository>();
            services.AddTransient<PriorityLevelRepository>();
            services.AddTransient<PriorityLevelService>();
            services.AddTransient<StatusRepository>();
            services.AddTransient<StatusService>();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {

                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddSingleton(Configuration);
            services.AddDbContext<PrioritizeDatabaseContext>(conf => conf.UseSqlServer(appConfig.PrioritizeItConnectionString));
            services.AddHttpContextAccessor();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddSessionStateTempDataProvider().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseSession();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
