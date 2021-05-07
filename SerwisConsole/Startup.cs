using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DP.Patients.Model;
using IdentityModel.Client;
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
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Logging;
using Serilog;


namespace DP.Patients
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            MainConsole();

        }
        List<string> dummy = new List<string>();// potrzebna do wywołania błędu
      

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers( options=> { options.RespectBrowserAcceptHeader = true; }).AddXmlDataContractSerializerFormatters();
            services.AddDbContext<DpDataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddApplicationInsightsTelemetry();
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

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
   

 /// <summary>
 /// //////////////////////////////////////////////////////////////////////////////////////////////////////
 /// </summary>
 /// 

        private static HttpClient client = new HttpClient();
        private string mainMicorServiceAdress = "https://localhost:44357/api/StudyData";

        // glowna funkcja wywolujaca po wykonaniu kazdej funkcji liste dostepnych operacji
        public async Task MainConsole()
        {
            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);

            //w celu inicjalizacji kontrollerów
            client.GetAsync("https://localhost:5003/api/serwisEwus");
            client.GetAsync("https://localhost:5008/api/serwisDb");

            client.GetAsync("https://localhost:5004/api/serwisCT");

            client.GetAsync("https://localhost:5007/api/serwisMRI");

            client.GetAsync("https://localhost:5001/api/SerwisOrg");
            client.GetAsync("https://localhost:5006/api/SerwisOrg");

            for (int i = 0; i < 100; i++)
            {
                Console.Write("wyswietl liste badan -l  dodaj badanie CT -ct  dodlaj badanie MRI - mri  - wyjatek e \n");
                mainConsoleFunction();
            }

            Console.ReadKey();
        }

        //obsluguje wydanie polecen do konsoli i tworzy podstawowy obiekt ktory nastepnie bedzie wyslany
        public async Task commonForStudies(string studyType)
        {
            Console.WriteLine("imie");
            var imie = Convert.ToString(Console.ReadLine());
            Console.WriteLine("nazwisko");
            var nazwisko = Convert.ToString(Console.ReadLine());
            Console.WriteLine("pesel");
            var pesel = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("czy pacjent ma w ciele elementy metaliczne  jesli tak wpisz t jesli nie wpisz n");
            var isFerroMagnetic = Convert.ToString(Console.ReadLine()) == "t";
            Console.WriteLine("czy zdiagnozowano u pacjenta niewydolnosc nerek");
            var isNiewydolNerek = Convert.ToString(Console.ReadLine()) == "t";
            Console.WriteLine("czy zdiagnozowano u pacjenta nadczynność tarczycy");
            var isNadczynnoscTarczycy = Convert.ToString(Console.ReadLine()) == "t";

            newStudySend(pesel, studyType, imie, nazwisko, isFerroMagnetic, isNiewydolNerek, isNadczynnoscTarczycy);

        }


        public async Task mainConsoleFunction()
        {

            switch (Console.ReadLine())
            {
                case "l":
                    Console.WriteLine("processing...");
                  var  str = await listStudies();
                    Console.WriteLine(str);

                    break;
                case "ct":

                    commonForStudies("CT");
                    break;
                case "mri":

                    commonForStudies("MRI");

                    break;
                case "e":
                    Console.WriteLine("wywolywanie bledu ...");
                    putToGetError();
                    break;
            }
        }

        // wysyla odpowiedni obiekt do kolejki w celu uzyskania informacji o liscie pacjentow
        public async Task<String> listStudies()
        {
            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);
              var list = await client.GetAsync("https://localhost:5008/api/serwisDb");
            var str = await list.Content.ReadAsStringAsync();
            return str;



        }

        // stworzone do sprawdzenia czy application insights dziala
        public async Task putToGetError()
        {
            client.GetAsync("https://localhost:5001/api/SerwisOrgt");

        }


        // wysyla za pośrednictwem protokolu HTTP
        public async Task newStudySend(int pesel, string studyType, string imie, string nazwisko, bool isFerroMagnetic, bool isNiewydolNerek, bool isNadczynnoscTarczycy)
        {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            // var app = PublicClientApplicationBuilder.Create("fce95216-40e5-4a34-b041-f287e46532be")
            //        .WithAuthority("https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/")
            //         .WithDefaultRedirectUri()
            //        .Build();

            //   var result =   await   app.AcquireTokenInteractive(new[] { "api://fce95216-40e5-4a34-b041-f287e46532be/.default" }).ExecuteAsync();

            serwisDbDat message = new serwisDbDat()
            {
                pesel = pesel,
                studyType = studyType,
                imie = imie,
                nazwisko = nazwisko,
                isFerroMagnetic = isFerroMagnetic,
                isNiewydolNerek = isNiewydolNerek,
                isNadczynnoscTarczycy = isNadczynnoscTarczycy
            };

            string studyJson = System.Text.Json.JsonSerializer.Serialize(message);
            client.PostAsync("https://localhost:5001/api/SerwisOrg", new StringContent(studyJson, Encoding.UTF8, "application/json"));
       

           


            Console.WriteLine("wyslane");
        }


        private async Task<string> GetToken()
        {
            using var client = new HttpClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/",
                Policy =
          {
          ValidateEndpoints = false
          }
            });

            if (disco.IsError)
                throw new InvalidOperationException(
                $"No discovery document. Details: {disco.Error}");

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "fce95216-40e5-4a34-b041-f287e46532be",
                ClientSecret = "XWGsyzt9uM-Ia2SA8WE7~gvUJ~4og-U_1p",
                Scope = "api://fce95216-40e5-4a34-b041-f287e46532be/.default"
            };

            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

            if (token.IsError)
                throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

            return token.AccessToken;
        }

    }


}
