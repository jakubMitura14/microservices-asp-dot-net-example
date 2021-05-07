using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using DP.Patients.Model;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;
using serwisDb.Model;
using IdentityModel.Client;
using Ctver3.Model;

namespace DP.Patients.NewFolder1
{
    public class ServiceBusManager
    {

        private Model.DpDataContext _context;
        private readonly QueueClient _queueClientserisGiveBack;


        private DpDataContext context;
        private static HttpClient client = new HttpClient();
        // gromadzi informacje na temat  danych wiadomości po peselu
        private Dictionary<int,List<Message>>  dictOfMessages = new Dictionary<int, List<Message>>();

        public IConfiguration Configuration { get; }

        public ServiceBusManager( IConfiguration configuration, DpDataContext context)
        {
            _queueClientserisGiveBack = new QueueClient(configuration.GetConnectionString("serwisGiveBackConnectionString"), "serwisgiveback");
            _context = context;
             Configuration = configuration;
        }



        // najpierw  rejestrujemy sie dokolejki i definiujemy która funkcja będzie wywołana gdy informacja dojdzie
        public async Task Register()
        {
            Console.WriteLine("RRRRRRRRRRRRRRRRRRRRRRRRRR egistered");

            var options = new MessageHandlerOptions((e) => Task.CompletedTask)
            {
                //trzeba rowniez pamietac zeby zamknac wiadomosc by znikneła z kolejki
                AutoComplete = true,
                MaxConcurrentCalls = 1

            };
            _queueClientserisGiveBack.RegisterMessageHandler(processMessage, options);
        }
        //jedynie do debugowania sprawdza jakie mamy klucze  w naszym słowniku (pesele) i jakie labels
        public string getPeselsAndLabels() {
            int  key = dictOfMessages.Keys.First();
            var lines = dictOfMessages.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
            return string.Join(Environment.NewLine, lines);
        }


        //tu definiujemy co zrobimy z wiadomością gdy zostanie ona odebrana z kolejki

        public async Task processMessage(Message message, CancellationToken token)

        {
            Console.Write("  processing message   ");
            //+ getPeselsAndLabels());
            Console.Write("  message.Label  " + message.Label);

            var pesel = getPeselFromMessage(message);
            Console.Write("  pesel " + pesel);

     

            // sprawdzamy czy w słowniku dany pesel już istnieje jako klucz jesli nie to tworzymy taki i przypisujemy mu nową listę
            if (!dictOfMessages.ContainsKey(pesel)) { dictOfMessages.Add(pesel, new List<Message>()); }
            // sprawdzmy czy lista zawierająca nasze message związane z danym peselem ma już jakąś o takim samym label jak label informacji przychodzącej (wtedy nie dodajemy jej do słownika
            var labelList = mapToLabels(dictOfMessages[pesel]); ;
            Console.Write("  mapToLabels " + labelList);

            if (!labelList.Contains(message.Label) ) {
            dictOfMessages[pesel].Add(message);
                    }
                        //po dodaniu informacji do kolejki sprawdzamy czy już mamy przynajmniej 3 labels
                        if (mapToLabels(dictOfMessages[pesel]).Count > 2) {
                            var messegeTosend = prepareOutputMessage(pesel);
                            sendToBeSaved(messegeTosend);
                            sendToConsole(messegeTosend);
               }
             

        }

        /**
         mapuje liste wiadomości do listy Label
         */
        private List<string> mapToLabels(List<Message> list) {
            var res = new List<string>();
            foreach (Message mess in list) {
                res.Add(mess.Label);
            }
            return res;
        }

   


        /**
         przygotowujemy tu wiadomość do zapisania i do wysłania za pomocą protokołu Http do konsoli
         */
        private serwisGiveBack prepareOutputMessage(int pesel) {

            // informacje przypisane do danego peselu
            var listOfMessages = dictOfMessages[pesel];

         int Pesel  = pesel;
         string imie="";
         string nazwisko="";
         string studyType="";
         bool isOktoNFZ=false;
         bool isSavedToPax= false;
         bool noContraIndications = false;

            foreach (var message in listOfMessages) {
                //tutaj musimy zdeserializować  w odpowiedni sposób przychodzącą wiadomość  zrobimy to na podstawie przypisanego label

                var label = message.Label;


                if (label == "serwisMRIDat")
                {
                    var payload = JsonConvert.DeserializeObject<serwisMRIDat>(Encoding.UTF8.GetString(message.Body));
                    studyType = "MRI";
                    noContraIndications = payload.isOktoMRI;
                }
                if (label == "serwisEwus")
                {   var payload = JsonConvert.DeserializeObject<serwisEwusDat>(Encoding.UTF8.GetString(message.Body));
                    isOktoNFZ = payload.isOkInNFZ;
                }
                if (label == "serwisDbDatToSend")
                {
                    var payload = JsonConvert.DeserializeObject<serwisDbDatToSend>(Encoding.UTF8.GetString(message.Body));
                    imie = payload.imie;
                    nazwisko = payload.nazwisko;
                    isSavedToPax = payload.isSaved;

                }
                if (label == "serwisCTDat")
                {
                    var payload = JsonConvert.DeserializeObject<serwisCTDat>(Encoding.UTF8.GetString(message.Body));
                    studyType = "CT";
                    noContraIndications = payload.isOktoCT;

                }



            }
            return new serwisGiveBack() {
                Pesel = Pesel,
                imie = imie,
                nazwisko = nazwisko,
                studyType = studyType,
                isOktoNFZ = isOktoNFZ,
                isSavedToPax = isSavedToPax,
                noContraIndications = noContraIndications

            };
        }


        /**
         pobiera informacje o peselu z Messege
         */
        private int getPeselFromMessage (Message message) {
            var label = message.Label;

            if (label == "serwisMRIDat")
            { var payload = JsonConvert.DeserializeObject<serwisMRIDat>(Encoding.UTF8.GetString(message.Body));   return payload.Pesel;

            }
            if (label == "serwisEwus")
            {  var payload = JsonConvert.DeserializeObject<serwisEwusDat>(Encoding.UTF8.GetString(message.Body));   return payload.Pesel;
            }
            if (label == "serwisDbDatToSend")
            {    var payload = JsonConvert.DeserializeObject<serwisDbDatToSend>(Encoding.UTF8.GetString(message.Body));  return payload.Pesel;
                            }
            if (label == "serwisCTDat")
            {
                var payload = JsonConvert.DeserializeObject<serwisCTDat>(Encoding.UTF8.GetString(message.Body));
                return payload.Pesel;

            }
            else return 0;

        }




        private async Task sendToBeSaved(serwisGiveBack dat)
        {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            client.PostAsync("https://localhost:5006/api/serwisOrg", new StringContent(studyJson, Encoding.UTF8, "application/json"));

        }

        private async Task sendToConsole(serwisGiveBack dat) {
            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            Console.WriteLine("seending to console  ");
            client.PostAsync("https://localhost:5002/api/Console", new StringContent(studyJson, Encoding.UTF8, "application/json"));
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
