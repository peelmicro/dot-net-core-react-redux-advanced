using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbGenericRepository;
using MongoDB.Bson;
using NetCoreReactReduxAdvanced.Models;
using NetCoreReactReduxAdvanced.Services;
using ServiceStack.Redis;
using Newtonsoft.Json.Linq;

namespace NetCoreReactReduxAdvanced
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
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // services.AddSession();

            var mongoDbContext = new MongoDbContext(
                Configuration.GetSection("MongoDbSettings").GetValue<string>("ConnectionString"),
                Configuration.GetSection("MongoDbSettings").GetValue<string>("DatabaseName")
            );
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddMongoDbStores<ApplicationUser, ApplicationRole, ObjectId>(mongoDbContext)
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                // Override the default events
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToAccessDenied = ReplaceRedirectorWithStatusCode(HttpStatusCode.Forbidden),
                    OnRedirectToLogin = ReplaceRedirectorWithStatusCode(HttpStatusCode.Unauthorized)
                };
            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration.GetSection("Google").GetValue<string>("ClientId");
                googleOptions.ClientSecret = Configuration.GetSection("Google").GetValue<string>("ClientSecret");
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });  
            services.AddSingleton<IRedisClientsManager> (c =>
                new RedisManagerPool(Configuration.GetSection("RedisSettings").GetValue<string>("Host")));

            services.AddSingleton<IMongoDbService, MongoDbService>();
            services.AddSingleton<IBlogService, BlogService>();
            services.AddScoped<IUserService, UserService>();

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
                if (env.IsProduction()) 
                {
                    app.UseHsts();
                    app.UseHttpsRedirection();
                }
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
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

        private static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirectorWithStatusCode(HttpStatusCode statusCode) => context =>
        {
            // Adapted from https://stackoverflow.com/questions/42030137/suppress-redirect-on-api-urls-in-asp-net-core
            context.Response.StatusCode = (int) statusCode;
            context.Response.ContentType = "application/json";
            dynamic result = new JObject();
            result.error = "You must log in!";
            context.Response.WriteAsync((string) result.ToString());
            return Task.CompletedTask;
        };
    }
}
