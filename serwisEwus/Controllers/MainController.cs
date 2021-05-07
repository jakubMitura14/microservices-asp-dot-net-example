using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DP.Patients.Model;
using DP.Patients.NewFolder1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using serwisEwus.Services;

namespace DP.Patients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class serwisEwusController : ControllerBase
    {
        public readonly DpDataContext _context;
        private readonly ILogger _logger;
        private IConfiguration _configuration ;
        private DbControll _dbContr;
        public serwisEwusController(DpDataContext context,  ILogger logger, IConfiguration configuration) {
            _logger = logger;
       
            _context = context;
            _configuration = configuration;
            _dbContr = new DbControll(context);
            var serviceBus = new ServiceBusManager(_dbContr, _configuration, _context);
            serviceBus.Register();

        }
        [HttpGet]
        public IActionResult getting()
        {

  

            return Ok("");
        }

        [HttpPost]
        public IActionResult Add(serwisEwusDat studyData)
        {
            _dbContr.saveToDb(studyData);

            return Ok("");
        }



    }




    }

    
 


