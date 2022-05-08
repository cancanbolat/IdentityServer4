using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarantiAPI
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

            #region Identity
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, op =>
                {
                    op.Authority = "https://localhost:1000";
                    op.Audience = "Garanti";
                    op.TokenValidationParameters = new TokenValidationParameters
                    {
                        LifetimeValidator = (notBefore, expires, tokenToValidate, tokenValidationParameters) => 
                            expires != null ? expires > DateTime.UtcNow : false
                    };
                });

            services.AddAuthorization(a =>
            {
                a.AddPolicy("ReadGaranti", policy => policy.RequireClaim("scope", "Garanti.Read"));
                a.AddPolicy("WriteGaranti", policy => policy.RequireClaim("scope", "Garanti.Write"));
                a.AddPolicy("ReadWriteGaranti", policy => policy.RequireClaim("scope", "Garanti.Read", "Garanti.Write"));
                a.AddPolicy("AllGaranti", policy => policy.RequireClaim("scope", "Garanti.All"));
                a.AddPolicy("AdminGaranti", policy => policy.RequireClaim("scope", "Garanti.Admin"));
            });
            #endregion

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GarantiAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GarantiAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
