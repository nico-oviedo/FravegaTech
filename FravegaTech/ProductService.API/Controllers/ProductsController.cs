using Microsoft.AspNetCore.Mvc;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        public ProductsController() { }


        [HttpGet("{sku}")]
        public string Get(string sku)
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
