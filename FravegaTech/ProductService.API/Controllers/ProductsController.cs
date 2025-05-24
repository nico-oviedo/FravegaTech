using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
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
        public async Task<IActionResult> GetAsync(string productId)
        {
            ProductDto productDto = await _productService.GetProductByIdAsync(productId);

            if (productDto is not null)
                return Ok(productDto);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ProductDto productDto)
        {
            string? productId = await _productService.GetProductIdOrInsertNewProductAsync(productDto);

            if (productId is not null)
                return Ok(productId);
            else
                return BadRequest();
        }
    }
}
