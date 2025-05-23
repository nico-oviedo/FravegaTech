using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
using Shared.Dtos;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductDto productDto)
        {
            string? productId = await _productService.GetOrInsertNewProduct(productDto);

            if (productId is not null)
                return Ok(productId);
            else
                return BadRequest();
        }
    }
}
