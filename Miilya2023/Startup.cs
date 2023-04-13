using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Miilya2023.Constants;
using Miilya2023.Middlewares;
using Miilya2023.Services.Abstract;
using Miilya2023.Services.Concrete;
using MongoDB.Driver;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Miilya2023
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            PrivateHistoryConstants.RootPath = configuration.GetValue<string>("PrivateHistoryRoot");
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IFamilyService, FamilyService>();
            services.AddSingleton<IUserAuthenticationService, UserAuthenticationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<PrivateHistoryMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(PrivateHistoryConstants.RootPath, "Media")),
                RequestPath = PrivateHistoryConstants.MediaUrlPrefix,
                ServeUnknownFileTypes = true
            });

            /*
            var key = GenerateRsaCryptoServiceProviderKey();
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            // Create JWT token with private key
            var securityKey = GenerateRsaCryptoServiceProviderKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Expires = DateTime.UtcNow.AddDays(7),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = credentials,
                IssuedAt = DateTime.UtcNow
            };

            var tokenHandler = new JwtSecurityTokenHandler
            {
                SetDefaultTimesOnTokenCreation = false
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            Console.WriteLine("JWT token: " + token);*/

            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        public SecurityKey GenerateRsaCryptoServiceProviderKey()
        {
            var rsaProvider = new RSACryptoServiceProvider(512);
            SecurityKey key = new RsaSecurityKey(rsaProvider);
            return key;
        }
    }
}
