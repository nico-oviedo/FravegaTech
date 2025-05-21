using Microsoft.AspNetCore.Mvc;

namespace EventService.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class EventsController : ControllerBase
    {
        public EventsController() { }


        [HttpGet("{id}")]
        public string Get(string id)
        {
            return null;
        }

        [HttpPost]
        public string Post()
        {
            return null;
        }
    }
}
