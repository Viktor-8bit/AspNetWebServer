using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using System.Security;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AspNetWebServer.Services;

using AspNetWebServer.Model.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

            services.AddHostedService<HeartbeatService>();
            
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();
            
            services.AddHttpContextAccessor();

            //Authorization && Authorization
            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            //Docs
            
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(
                        "http://185.104.114.7:5173", 
                        "http://localhost:5173", 
                        "http://185.104.114.7", 
                        "http://localhost:8001", 
                        "http://10.66.24.0",
                        "http://172.17.0.4:8000",
                        "http://172.17.0.5:5173",
                        "http://185.104.114.7:8001"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
                /*
                options.AddPolicy("AuthPolicy", builder =>
                {
                    builder
                        .WithOrigins("http://localhost:5173", "185.104.114.7", "http://localhost:8000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }); */
            });
           
                /*
            services.AddCors(options =>
            {
                options.AddPolicy(name: "MyPolicy",
                    policy =>
                    {
                        policy.WithOrigins("*",
                                "http://localhost:5173/")
                            .WithMethods("PUT", "DELETE", "GET");
                    });
            });
            */
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
            //app.UseCors("AuthPolicy");

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseCors();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "api/{controller}/{action}/{id?}");
            });
        }

    }
}
