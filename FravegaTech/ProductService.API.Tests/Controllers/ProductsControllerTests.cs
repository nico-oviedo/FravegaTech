using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.API.Controllers;
using ProductService.Application.Services;
using SharedKernel.Dtos;
using SharedKernel.Exceptions;

namespace ProductService.API.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _productsController;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _productsController = new ProductsController(_mockProductService.Object, _mockLogger.Object);
        }

        #region GetAsync

        [Fact]
        public async Task GetAsync_ReturnsOk_WhenProductExists()
        {
            var productId = "345BVC";
            var productDto = new ProductDto { SKU = "P134", Name = "Heladera", Price = 10500 };
            _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
                .ReturnsAsync(productDto);

            var result = await _productsController.GetAsync(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(productDto, okResult.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsBadRequest_WhenIdIsNullOrEmpty()
        {
            var result = await _productsController.GetAsync("");
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id del producto es requerido.", badRequest.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound_WhenProductNotFound()
        {
            _mockProductService.Setup(s => s.GetProductByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new NotFoundException("Product", "ProductService"));

            var result = await _productsController.GetAsync("TR123");
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Producto no fue encontrado.", notFound.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenUnhandledException()
        {
            _mockProductService.Setup(s => s.GetProductByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var result = await _productsController.GetAsync("BB345");
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Un error interno ha ocurrido.", serverError.Value);
        }

        #endregion

        #region PostAsync

        [Fact]
        public async Task PostAsync_ReturnsOk_WhenProductAdded()
        {
            var productDto = new ProductDto { SKU = "P134", Name = "Heladera", Price = 10500 };
            _mockProductService.Setup(s => s.AddProductAsync(productDto)).ReturnsAsync("ABC456");

            var result = await _productsController.PostAsync(productDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("ABC456", okResult.Value);
        }

        [Fact]
        public async Task PostAsync_Returns500_WhenServiceThrows()
        {
            var productDto = new ProductDto { SKU = "P134", Name = "Heladera", Price = 10500 };
            _mockProductService.Setup(s => s.AddProductAsync(productDto)).ThrowsAsync(new DataAccessException("productService", new Exception()));

            var result = await _productsController.PostAsync(productDto);
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Ocurrió un error al ingresar un nuevo producto en el sistema.", serverError.Value);
        }

        #endregion
    }
}
