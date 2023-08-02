using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using HeritageSite.Constants;
using HeritageSite.Middlewares;
using HeritageSite.Services.Abstract;
using HeritageSite.Services.Concrete;
using System.IO;
using static HeritageSite.Services.Utils.Documents;
using static HeritageSite.Services.Utils.DocumentsExternal;
using Neo4j.Driver;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace HeritageSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            PrivateHistoryConstants.RootPath = configuration.GetValue<string>("PrivateHistoryRoot");
            PrivateHistoryConstants.ImagesDirectoryPath = Path.Combine(PrivateHistoryConstants.RootPath, "Media", "Images");
            PrivateHistoryConstants.FamiliesDirectoryPath = Path.Combine(PrivateHistoryConstants.RootPath, "Families");
            AuthenticationConstants.MicrosoftClientSecret = configuration.GetValue<string>("MicrosoftClientSecret");
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.CreateMap<UserDocument, UserDocumentExternal>();
                mc.CreateMap<FamilyDocument, FamilyDocumentExternal>();
                mc.CreateMap<HistoryPostDocument, HistoryPostDocumentExternal>();
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddSingleton<IHistoryPostService, HistoryPostServiceNeo>();
            services.AddSingleton<IFamilyService, FamilyService>();
            services.AddSingleton<IUserAuthenticationService, UserAuthenticationService>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton(provider =>
            {
                return GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "123123123"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CustomExceptionHandlerMiddleware>();
            app.UseMiddleware<PrivateHistoryMiddleware>();

            if (env.IsDevelopment())
            {
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".dzi"] = "application/octet-stream";
            provider.Mappings[".jpg"] = "image/jpeg";

            var privateHistoryMediaPath = Path.Combine(PrivateHistoryConstants.RootPath, "Media");
            Directory.CreateDirectory(privateHistoryMediaPath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(privateHistoryMediaPath),
                RequestPath = PrivateHistoryConstants.MediaUrlPrefix,
                ServeUnknownFileTypes = false,
                ContentTypeProvider = provider,
            });

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
    }
}
