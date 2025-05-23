using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
using ProductService.Domain;
using SharedKernel.Dtos;

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

        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(string productId)
        {
            Product? product = await _productService.GetProductByIdAsync(productId);

            if (product is not null)
                return Ok(product);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductDto productDto)
        {
            string? productId = await _productService.GetOrInsertNewProductAsync(productDto);

            if (productId is not null)
                return Ok(productId);
            else
                return BadRequest();
        }
    }
}
