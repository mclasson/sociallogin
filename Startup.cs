using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Mvc;

namespace customlogin
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
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookieAuthentication(CookieAuthenticationDefaults.AuthenticationScheme, option =>
            {
                option.LoginPath = "/home/login";
            })
            .AddTwitterAuthentication(o =>
            {
                o.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                o.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];

            });
            
            services.AddMvc();
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var options = new RewriteOptions().AddRedirectToHttps(301, 44326);
            app.UseRewriter(options);

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.Map("/login-twitter", login =>
             {
                 login.Run(async context =>
                 {
                     
                     await context.ChallengeAsync("Twitter", new AuthenticationProperties() {
                         RedirectUri = "/" });
                     return;
                 });
             });


        }
    }
}