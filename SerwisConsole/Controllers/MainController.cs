using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DP.Patients.Model;
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

    public class ConsoleController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ILogger _logger;
        public ConsoleController(DpDataContext context,  ILogger logger) {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public IActionResult Add(serwisGiveBack studyData)
        {
            Console.WriteLine(" receiveeed ");
            Console.WriteLine("rodzaj badania " + studyData.studyType);
            Console.WriteLine("imie "+ studyData.imie);
            Console.WriteLine("nazwisko "+ studyData.nazwisko);
            Console.WriteLine("pesel "+ studyData.Pesel);
            Console.WriteLine("pacjent w Ewus "+ studyData.isOktoNFZ);
            Console.WriteLine("badanie zapisane "+ studyData.isSavedToPax);
            Console.WriteLine("brak przeciwskazan do badania "+ studyData.noContraIndications);
            return Ok("");
        }

    }

    }
 


