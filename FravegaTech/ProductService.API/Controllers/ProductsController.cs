using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
using SharedKernel.Dtos;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        /// <summary>
        /// Gets product dto by id
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>Product dto object.</returns>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAsync(string productId)
        {
            ProductDto productDto = await _productService.GetProductByIdAsync(productId);

            if (productDto is not null)
                return Ok(productDto);
            else
                return NotFound();
        }

        /// <summary>
        /// Adds new product
        /// </summary>
        /// <param name="productDto">Product dto object.</param>
        /// <returns>Added product id.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ProductDto productDto)
        {
            string? productId = await _productService.AddProductAsync(productDto);

            if (productId is not null)
                return Ok(productId);
            else
                return BadRequest();
        }
    }
}
