using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Data.Repositories;
using ProductService.Domain;
using SharedKernel.Dtos;
using SharedKernel.Exceptions;
using ProductApplication = ProductService.Application.Services;

namespace ProductService.Application.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ProductApplication.ProductService>> _mockLogger;
        private readonly ProductApplication.ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ProductApplication.ProductService>>();
            _productService = new ProductApplication.ProductService(_mockProductRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsProductDto_WhenProductExists()
        {
            var productId = "ABC123";
            var product = new Product { _id = productId, SKU = "P134", Name = "Lavarropas", Price = 12500 };
            var productDto = new ProductDto { SKU = "P134", Name = "Lavarropas", Price = 12500 };

            _mockProductRepository.Setup(r => r.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

            var result = await _productService.GetProductByIdAsync(productId);

            Assert.NotNull(result);
            Assert.Equal(product.SKU, result.SKU);
        }

        [Fact]
        public async Task GetProductByIdAsync_ThrowsNotFoundException_WhenProductDoesNotExist()
        {
            var productId = "HGF334";
            _mockProductRepository.Setup(r => r.GetProductByIdAsync(productId)).ReturnsAsync((Product)null!);

            await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetProductByIdAsync(productId));
        }

        [Fact]
        public async Task AddProductAsync_ReturnsExistingProductId_WhenAlreadyExists()
        {
            var productDto = new ProductDto { SKU = "P134", Name = "Lavarropas", Price = 12500 };
            _mockProductRepository.Setup(r => r.GetProductIdBySKUAsync(productDto.SKU)).ReturnsAsync("KJG456");

            var result = await _productService.AddProductAsync(productDto);

            Assert.Equal("KJG456", result);
            _mockProductRepository.Verify(r => r.AddProductAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task AddProductAsync_AddsProductAndReturnsNewId_WhenNotExists()
        {
            var productDto = new ProductDto { SKU = "P134", Name = "Lavarropas", Price = 12500 };
            var product = new Product { _id = "AAA027", SKU = "P134", Name = "Lavarropas", Price = 12500 };

            _mockProductRepository.Setup(r => r.GetProductIdBySKUAsync(productDto.SKU)).ReturnsAsync((string)null!);
            _mockMapper.Setup(m => m.Map<Product>(productDto)).Returns(product);
            _mockProductRepository.Setup(r => r.AddProductAsync(product)).ReturnsAsync("AAA027");

            var result = await _productService.AddProductAsync(productDto);

            Assert.Equal("AAA027", result);
        }
    }
}
