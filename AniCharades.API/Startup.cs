using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JikanDotNet;
using Newtonsoft.Json;
using AniCharades.Data.Context;
using AniCharades.Repositories.Interfaces;
using AniCharades.Repositories.Implementation;
using AniCharades.Algorithms.MyAnimeList;
using AniCharades.Services.Implementation;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Franchise;
using AniCharades.Services.Providers;

namespace AniCharades.API
{
    public class Startup
    {
        private static readonly string DbConnectionString = "DefaultConnection";
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            services.AddDbContext<DataContext>(
                x => x.UseSqlite(Configuration.GetConnectionString(DbConnectionString), 
                b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)));
            services.AddScoped<ISeriesRepository, SeriesRepository>();
            services.AddScoped<ICharadesCompositionService, CharadesCompositionService>();
            services.AddScoped<IMyAnimeListService, MyAnimeListService>();
            services.AddScoped<IJikan>(j => new Jikan(true));
            services.AddScoped<IListExtractor, ListExtractor>();
            services.AddScoped<FranchiseAssembler, FranchiseAssembler>();
            services.AddScoped<JikanAnimeProvider, JikanAnimeProvider>();
            services.AddScoped<JikanMangaProvider, JikanMangaProvider>();
            services.AddScoped<IFranchiseService, FranchiseService>();
            services.AddScoped<IRelationService, RelationService>();
            services.AddAWSService<Amazon.S3.IAmazonS3>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
