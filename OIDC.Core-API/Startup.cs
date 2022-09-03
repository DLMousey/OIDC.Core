using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OAuthServer.DAL;
using OAuthServer.Services.Implementation;
using OAuthServer.Services.Interface;
using Newtonsoft.Json;
using OAuthServer.Middleware;
using YamlDotNet;

namespace OAuthServer
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
            services.AddControllers().AddNewtonsoftJson(x => 
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            // Scoped services
            services.AddScoped<IAccessTokenService, AccessTokenService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IAuthorisationCodeService, AuthorisationCodeService>();
            services.AddScoped<IRandomValueService, RandomValueService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IScopeService, ScopeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserApplicationService, UserApplicationService>();
            services.AddScoped<IJwtService, JwtService>();
            
            // Singletons
            services.AddSingleton<IEmailService, EmailService>();

            services.AddCors(options =>
            {
                options.AddPolicy("frontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
                
                options.AddPolicy("open", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            
            // app.UseAuthorization();

            app.UseCors("frontend");
            app.UseCors("open");

            app.UseMiddleware<VerifyAccessToken>();
            // app.UseMiddleware<Pyromaniac.Pyromaniac>();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}