﻿using Microsoft.Extensions.Configuration;
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
using IdentityModel.Client;
using serwisDb.Model;

namespace DP.Patients.NewFolder1
{
    public class ServiceBusManager
    {

        private readonly QueueClient _queueClientSerwisOrg;
        private Model.DpDataContext _context;
        private readonly QueueClient _queueClientserisGiveBack;


        private DpDataContext context;
        private static HttpClient client = new HttpClient();

        public IConfiguration Configuration { get; }

        public ServiceBusManager( IConfiguration configuration, DpDataContext context)
        {
            _queueClientserisGiveBack = new QueueClient(configuration.GetConnectionString("serwisGiveBackConnectionString"), "serwisgiveback");
            _queueClientSerwisOrg = new QueueClient(configuration.GetConnectionString("SerwisOrgConnectionString"), "serwisorg");
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
                AutoComplete = true
            };
            _queueClientSerwisOrg.RegisterMessageHandler(processMessage, options);
        }
        //tu definiujemy co zrobimy z wiadomością gdy zostanie ona odebrana z kolejki

        public async Task processMessage(Message message, CancellationToken token)

        {
            Console.WriteLine("information processsing ");
            var payload = JsonConvert.DeserializeObject<serwisDbDat>(Encoding.UTF8.GetString(message.Body));
           await sendToBeSaved(payload);
            // wysylamy do kolejnej kolejki w celu dalszego przetworzenia informacji

            await sendToGiveBack(payload);


        }



        private async Task sendToBeSaved(serwisDbDat dat)
        {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            client.PostAsync("https://localhost:5008/api/serwisDb", new StringContent(studyJson, Encoding.UTF8, "application/json"));

        }



        /**
  wysylamy do kolejki GiveBack 
  */
        public async Task sendToGiveBack(serwisDbDat payload)
        {

            var dat = new serwisDbDatToSend()
            {
                Pesel = payload.pesel,

                imie = payload.imie,

                nazwisko = payload.nazwisko,

                isSaved = true

            };

            string data = JsonConvert.SerializeObject(dat);

            Message message = new Message(Encoding.UTF8.GetBytes(data));

            message.Label = "serwisDbDatToSend"; // ustawiamy odpowiedni Label  by później móc odpowiednio zdeserializować ten obiekt w GiveBack




            await _queueClientserisGiveBack.SendAsync(message);

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
