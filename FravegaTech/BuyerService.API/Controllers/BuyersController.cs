using Microsoft.AspNetCore.Mvc;

namespace BuyerService.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class BuyersController : ControllerBase
    {
        public BuyersController() { }


        [HttpGet("{documentNumber}")]
        public string Get(string documentNumber)
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
