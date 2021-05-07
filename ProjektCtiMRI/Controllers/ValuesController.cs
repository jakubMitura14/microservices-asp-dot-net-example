using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client;
using Microsoft.AspNetCore.Mvc;
using ProjektCtiMRI.Modules;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjektCtiMRI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudyDataController : ControllerBase
    {
        private readonly CTiMRIDataContext context;
        private readonly ServiceBusSenderr sender;

        public StudyDataController(CTiMRIDataContext context, ServiceBusSenderr sender)
        {
            this.context = context;
            this.sender = sender;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            return Ok(context.CTiMRI.ToList());
        }
              

        [HttpPost]
        public IActionResult AddStudy(CTiMRI stud)
        {
            Console.WriteLine("Adding Study");
            sender.SendMessage(new MessegePayload() { 
            messageTitle = "test message of Mitura"});
            context.CTiMRI.Add(stud);
            context.SaveChanges();
            return Ok("added");
        }

     
    }
}
