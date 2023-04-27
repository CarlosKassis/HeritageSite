using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Miilya2023.Constants;
using Miilya2023.Middlewares;
using Miilya2023.Services.Abstract;
using Miilya2023.Services.Concrete;
using System.IO;
using static Miilya2023.Services.Utils.Documents;
using static Miilya2023.Services.Utils.DocumentsExternal;

namespace Miilya2023
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            PrivateHistoryConstants.RootPath = configuration.GetValue<string>("PrivateHistoryRoot");
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

            services.AddSingleton<IHistoryPostService, HistoryPostService>();
            services.AddSingleton<IFamilyService, FamilyService>();
            services.AddSingleton<IUserAuthenticationService, UserAuthenticationService>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IBookmarkService, BookmarkService>();
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
            // TODO: Split to two serves, images, and dzi files
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(PrivateHistoryConstants.RootPath, "Media")),
                RequestPath = PrivateHistoryConstants.MediaUrlPrefix,
                ServeUnknownFileTypes = true
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
