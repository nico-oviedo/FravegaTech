using Microsoft.AspNetCore.Mvc;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        public OrdersController() { }


        [HttpGet("{orderId}")]
        public string Get(int orderId)
        {
            return null;
        }

        [HttpGet("search")]
        public string Get()
        {
            return null;
        }

        [HttpPost]
        public string Post() //FromBody Order
        {
            return null;
        }

        [HttpPost("{orderId}/events")]
        public string Post(int orderId) //FromBody evento
        {
            return null;
        }
    }
}
