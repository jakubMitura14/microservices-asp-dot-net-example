using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ctver3.Model;
using DP.Patients.Model;
using DP.Patients.NewFolder1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;

namespace DP.Patients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class serwisMRIController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ServiceBusManager _sender;
        private readonly ILogger _logger;

        public serwisMRIController(IConfiguration configuration,DpDataContext context,  ILogger logger) {
            _logger = logger;
            _sender = new ServiceBusManager(configuration,context);
            _sender.Register();
            _context = context;

        }

     
        [HttpPost]
        public IActionResult Add(serwisMRIDat studyData)
        {
            _context.serwisMRIDat.Add(studyData);
            _context.SaveChanges();
            return Ok("");
        }

        [HttpGet]
        public IActionResult getting()
        {



            return Ok("");
        }
    }

}
 


