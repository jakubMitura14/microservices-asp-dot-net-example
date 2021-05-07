using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ServiceBusManager
    {
        private readonly QueueClient queeueClient ;
        public ServiceBusManager(IConfiguration iconfiguration)
        {
            this.queeueClient = new  QueueClient(iconfiguration.GetConnectionString("BusConnection"),"messages");
        }

        public async Task SendMessage(MessegePayload payload) {
            string data = JsonConvert.SerializeObject(payload);
            Message message = new Message(Encoding.UTF8.GetBytes(data));
            await queeueClient.SendAsync(message);
        }
    }
}
public class MessegePayload
{
    public string messageTitle { get; set; }
    public string studyType { get; set; }
    public string imie { get; set; }
    public string nazwisko { get; set; }
    public bool isFerroMagnetic { get; set; }// prawda jesli sa ferromagnetyczne elementy metalowe w ciele
    public bool isNiewydolNerek { get; set; } // prawda jesli pacjent ma niewydolnosc nerek
    public bool isNadczynnoscTarczycy { get; set; }// prawda jesli pacjent ma nadczynnosc tarczycy

}


