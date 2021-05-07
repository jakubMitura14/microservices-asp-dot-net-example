using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DP.Patients.Controllers;
using DP.Patients.Model;
using DP.Patients.NewFolder1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Serilog;

namespace DP.Patients
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
            // w celu wywołania konstruktora i zarejestrowania Service bus managera podczas uruchamiania programu dlatego doadalismy AddControllersAsServices


            services.AddDbContext<DpDataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers( options=> { options.RespectBrowserAcceptHeader = true; }).AddXmlDataContractSerializerFormatters().AddControllersAsServices();
           
            
            
          // services.AddScoped<DpDataContext>();

            services.AddApplicationInsightsTelemetry();
          //  services.AddSingleton<ServiceBusManager>();




            services.AddAuthentication(options=>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;            
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;            
            }).AddJwtBearer(options=>
            {
                options.Authority = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/";
                options.Audience = "api://fce95216-40e5-4a34-b041-f287e46532be";
                options.TokenValidationParameters.ValidateIssuer = false;

                options.IncludeErrorDetails = true;

            } );

     
            IdentityModelEventSource.ShowPII = true;
            services.AddSingleton(_ => Log.Logger);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
             app.UseHttpsRedirection();

            // w celu wywołania konstruktora i zarejestrowania Service bus managera podczas uruchamiania programu jako że wcześniej dodalismy kontrolery jako serwis teraz możemy go tu wywołać
           
            
          //  var serviceBus = app.ApplicationServices.GetService<ServiceBusManager>();
         //  serviceBus.Register();

          //  var dbDataContext = app.ApplicationServices.GetService<serwisEwusController>();
          //  serviceBus.Register(dbDataContext);




            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
