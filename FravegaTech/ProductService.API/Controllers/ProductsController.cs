using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
using SharedKernel.Dtos;
using SharedKernel.Exceptions;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets product dto by id
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>Product dto object.</returns>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAsync(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                    return BadRequest("Id del producto es requerido.");

                _logger.LogInformation($"START endpoint call {GetType().Name}:{nameof(GetAsync)}.");
                ProductDto productDto = await _productService.GetProductByIdAsync(productId);

                _logger.LogInformation($"END endpoint call {GetType().Name}.{nameof(GetAsync)}.");
                return Ok(productDto);
            }
            catch (NotFoundException)
            {
                return NotFound("Producto no fue encontrado.");
            }
            catch (DataAccessException)
            {
                return StatusCode(500, "Ocurrió un error al obtener los datos del producto.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Un error interno ha ocurrido.");
            }
        }

        /// <summary>
        /// Adds new product
        /// </summary>
        /// <param name="productDto">Product dto object.</param>
        /// <returns>Added product id.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ProductDto productDto)
        {
            try
            {
                _logger.LogInformation($"START endpoint call {GetType().Name}:{nameof(PostAsync)}.");
                string productId = await _productService.AddProductAsync(productDto);

                _logger.LogInformation($"END endpoint call {GetType().Name}.{nameof(PostAsync)}.");
                return Ok(productId);
            }
            catch (DataAccessException)
            {
                return StatusCode(500, "Ocurrió un error al ingresar un nuevo producto en el sistema.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Un error interno ha ocurrido.");
            }
        }
    }
}
