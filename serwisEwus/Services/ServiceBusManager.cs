using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using DP.Patients.Services;
using System.Threading;
using serwisEwus.Services;
using DP.Patients.Model;
using System.Net.Http;
using IdentityModel.Client;
using System.Net.Http.Headers;

namespace DP.Patients.NewFolder1
{
    public class ServiceBusManager
    {
    
        private readonly QueueClient _queueClientSerwisEwus;
        private readonly QueueClient _queueClientserisGiveBack;
        private Model.DpDataContext _context;
        private DbControll _dbContr;
        private DpDataContext context;
        private static HttpClient client = new HttpClient();

        public IConfiguration Configuration { get; }

        public ServiceBusManager(DbControll dbContr, IConfiguration configuration)
        {
            _queueClientserisGiveBack = new QueueClient(configuration.GetConnectionString("serwisGiveBackConnectionString"), "serwisgiveback");
            _queueClientSerwisEwus = new QueueClient(configuration.GetConnectionString("SerwisEwusConnectionString"), "serwisewus");

            _dbContr = dbContr;
            Configuration = configuration;
        }

        public ServiceBusManager(DbControll dbContr, IConfiguration configuration, DpDataContext context) : this(dbContr, configuration)
        {
            _context = context;
        }


        // najpierw  rejestrujemy sie dokolejki i definiujemy która funkcja będzie wywołana gdy informacja dojdzie
        public async Task  Register()
        {



            Console.WriteLine("RRRRRRRRRRRRRRRRRRRRRRRRRR egistered");

            var options = new MessageHandlerOptions((e) => Task.CompletedTask)
            {
                 //trzeba rowniez pamietac zeby zamknac wiadomosc by znikneła z kolejki
                      AutoComplete = true
            };
             _queueClientSerwisEwus.RegisterMessageHandler( processMessage, options);
        }
        //tu definiujemy co zrobimy z wiadomością gdy zostanie ona odebrana z kolejki

        public async Task processMessage(Message message, CancellationToken token)

        {
     

            Console.WriteLine("processssssssssssssssssssssing");



            // deserializacja JSON w obiekt C# serwisDbDat
            var payload = JsonConvert.DeserializeObject<serwisDbDat>(Encoding.UTF8.GetString(message.Body));

            // tu wywołamy fukcje sprawdzajaca czy pacjent jest w Ewus
            var dummyCheck = new DummyEwusCheck();

            var isEwusOk = dummyCheck.checkEwus(payload).Result;

            //stworzenie obiektu który z jednej strony będzie zapisywany do bazy danych a z drugiej wysyłany do kolejnej kolejki giveItBackService
            var dat = new serwisEwusDat() {
            Pesel =  payload.pesel, 
                
             imie =    payload.imie, 
                
                nazwisko = payload.nazwisko,

                isOkInNFZ = isEwusOk

            };


            // wysylamy do kolejnej kolejki w celu dalszego przetworzenia informacji
        await sendToGiveBack(dat);


            sendToBeSaved(dat);
       


        }




        /**
         wysylamy do kolejki GiveBack 
         */
        public async Task sendToGiveBack(serwisEwusDat dat) {

            string data = JsonConvert.SerializeObject(dat);

            Message message = new Message(Encoding.UTF8.GetBytes(data));

            message.Label = "serwisEwus"; // ustawiamy odpowiedni Label  by później móc odpowiednio zdeserializować ten obiekt w GiveBack

            await _queueClientserisGiveBack.SendAsync(message);

        }








        private async Task sendToBeSaved(serwisEwusDat dat) {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


                     string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            client.PostAsync("https://localhost:5003/api/serwisEwus", new StringContent(studyJson, Encoding.UTF8, "application/json"));

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
