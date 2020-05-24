
using MedicamentsAggregator.Service.DataLayer.Context;
using MedicamentsAggregator.Service.Models.Aggregate;
using MedicamentsAggregator.Service.Models.Common;
using MedicamentsAggregator.Service.Models.GeoCoder;
using MedicamentsAggregator.Service.Models.Logs;
using MedicamentsAggregator.Service.Models.Medgorodok;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vostok.Logging.Microsoft;
using VueCliMiddleware;

namespace MedicamentsAggregator.Service
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

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
            services.AddHttpClient();
            services.AddDbContext<MedicamentsAggregatorContext>(options =>
                options
                    .UseNpgsql(Configuration.GetConnectionString("MedicamentsAggregator"))
                    .UseLoggerFactory(EntityFrameworkLoggerFactory.Singleton));
            services.AddSingleton<MedicamentsAggregatorContextFactory>();
            services.AddSingleton<Repository>();
            
            services.AddScoped<AggregateProcessor>();
            services.AddScoped<MedgorodokMedicamentPageParser>();
            services.AddSingleton<MedgorodokLog>();
            services.AddSingleton<GeoCoderLog>();
            services.AddScoped<PharmacyCoordinatesUpdater>();
            services.AddScoped<Aggregator>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, MedicamentsAggregatorContextFactory contextFactory)
        {
            UpdateDatabase(contextFactory);
            loggerFactory.AddVostok(new ApplicationLog());
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
                    spa.UseVueCli();
                }
            });
        }

        private void UpdateDatabase(MedicamentsAggregatorContextFactory contextFactory)
        {
            using (var context = contextFactory.CreateContext())
            {
                context.Database.Migrate();
            }
        }
    }
}