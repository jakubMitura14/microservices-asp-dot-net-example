using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DP.Patients.Model;
using DP.Patients.NewFolder1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;

namespace DP.Patients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SerwisOrgController : ControllerBase
    {
        private readonly ServiceBusManager _sender;
        private readonly ILogger _logger;
        public SerwisOrgController( ServiceBusManager sender, ILogger logger) {
            _logger = logger;
            _sender = sender;
        }

        [HttpPost]
        public IActionResult Add(serwisDbDat studyData)
        {
            //wysylanie do odpowiedni=ch kolejek
            _sender.SendMessage(studyData);

  
            return Ok("");
        }



    }


}

    
 


