using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthClientMvc
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

            services.AddAuthentication(a =>
            {
                a.DefaultScheme = "AuthClientMvcCookie";
                a.DefaultChallengeScheme = "oidc";
            })
            //Kullanıcının rolünün olmadığı sayfaya geldiğinde buraya yönlendirme yapacağız.
            .AddCookie("AuthClientMvcCookie", options => options.AccessDeniedPath = "home/accessdenied")
            .AddOpenIdConnect("oidc", o =>
            {
                o.SignInScheme = "AuthClientMvcCookie";
                o.Authority = "https://localhost:1000";
                o.ClientId = "AuthClientMvc";
                o.ClientSecret = "authclientmvc";
                o.ResponseType = "code id_token";
                o.GetClaimsFromUserInfoEndpoint = true; //User claimlerini almak için

                o.SaveTokens = true; //Access token için
                o.Scope.Add("offline_access"); //refresh token için

                o.Scope.Add("Garanti.Write");
                o.Scope.Add("Garanti.Read");
                o.Scope.Add("PositionAndAuthority");
                o.Scope.Add("Roles");

                o.ClaimActions.MapUniqueJsonKey("position", "position");
                o.ClaimActions.MapUniqueJsonKey("authority", "authority");
                o.ClaimActions.MapUniqueJsonKey("role", "role");

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = "role"
                };
            });
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
