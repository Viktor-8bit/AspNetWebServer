using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using System.Security;

using AspNetWebServer.Model.Data;

namespace AspNetWebServer
{
    public class Startup
    {

        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options => options.AddServerHeader = false);
            //TODO: Swagger 401 model
            //services.Configure<ApiBehaviorOptions>(options => options.);


            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();

            //Authorization


            //Docs
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });



            //Add Caching
            services.AddMemoryCache();



            //Add database
            services.AddDbContext<ApplicationContext>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("/index.html");
                endpoints.MapControllerRoute(
                    "default",
                    "api/{controller}/{action}/{id?}");
            });
        }

    }
}
