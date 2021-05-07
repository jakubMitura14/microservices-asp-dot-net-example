using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client;
using Microsoft.AspNetCore.Mvc;


namespace MRI
{
    [Route("api/[controller]")]
    [ApiController]
    public class CTController : ControllerBase
    {

        private readonly ServiceBusManager busManag;

        public CTController(ServiceBusManager busManag)
        {
            this.busManag = busManag;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            return Ok("");
        }


        [HttpPost]
        public IActionResult AddData()
        {

            return Ok("added");
        }


    }
}
