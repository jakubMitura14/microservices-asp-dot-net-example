using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using DP.Patients.Model;

namespace DP.Patients.NewFolder1
{
    public class ServiceBusManager
    {
        private readonly QueueClient _queueClientSerwisOrg;
        private readonly QueueClient _queueClientSerwisEwus;
        private readonly QueueClient _queueClientserwisCTver3;
        private readonly QueueClient _queueClientserwisMRI;

        // sprawdzic czy informacja jest w kolejce wyklad 2 1:09:37
        public ServiceBusManager(IConfiguration configuration) {
            _queueClientSerwisOrg = new QueueClient(configuration.GetConnectionString("SerwisOrgConnectionString"), "serwisorg");
            _queueClientSerwisEwus = new QueueClient(configuration.GetConnectionString("SerwisEwusConnectionString"), "serwisewus");
            _queueClientserwisCTver3 = new QueueClient(configuration.GetConnectionString("serwisCTver3ConnectionString"), "serwisctver3");
            _queueClientserwisMRI = new QueueClient(configuration.GetConnectionString("serwisMRIconnectionString"), "serwismri");
        }

        public async Task SendMessage(serwisDbDat payload)
        {
            string data = JsonConvert.SerializeObject(payload);
            Message message = new Message(Encoding.UTF8.GetBytes(data));
           await  _queueClientSerwisOrg.SendAsync(message);
            await _queueClientSerwisEwus.SendAsync(message);

            // musimy kontrolowac gdzie to wysyłamy w zależności od rodzaju badania
            var rodzajBad = payload.studyType;
            if (rodzajBad== "CT") {
                await _queueClientserwisCTver3.SendAsync(message);
            }
            if (rodzajBad == "MRI")
            {
                await _queueClientserwisMRI.SendAsync(message);
            }
            }
    }



}
