using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using ProductService.Data.Repositories;
using ProductService.Domain;
using SharedKernel.Exceptions;

namespace ProductService.Data.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly Mock<ILogger<ProductRepository>> _mockLogger = new();
        private readonly Mock<IMongoCollection<Product>> _mockProductsCollection = new();
        private readonly ProductRepository _productRepository;

        public ProductRepositoryTests()
        {
            var configValues = new Dictionary<string, string?>
            {
                {"ConnectionStrings:MongoDB", "mongodb://localhost:27017"},
                {"ConnectionStrings:ProductDatabase", "productDb"},
                {"ConnectionStrings:ProductsCollection", "products"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _productRepository = new ProductRepository(configuration, _mockLogger.Object);

            typeof(ProductRepository)
                .GetField("_products", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(_productRepository, _mockProductsCollection.Object);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsProduct_WhenFound()
        {
            var product = new Product { _id = "FRE123", SKU = "P134", Name = "Playstation 5", Price = 108000 };

            var mockAsyncCursor = new Mock<IAsyncCursor<Product>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Product> { product });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockProductsCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            var result = await _productRepository.GetProductByIdAsync("FRE123");

            Assert.Equal(product, result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ThrowsDataAccessException_OnError()
        {
            var product = new Product { _id = "FRE123", SKU = "P134", Name = "Playstation 5", Price = 108000 };

            var mockAsyncCursor = new Mock<IAsyncCursor<Product>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Product> { product });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockProductsCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Data access exception."));

            await Assert.ThrowsAsync<DataAccessException>(() => _productRepository.GetProductByIdAsync("FRE123"));

        }

        [Fact]
        public async Task GetProductIdByDocumentNumberAsync_ReturnsId_WhenProductExists()
        {
            var product = new Product { _id = "FRE123", SKU = "P134", Name = "Playstation 5", Price = 108000 };

            var mockAsyncCursor = new Mock<IAsyncCursor<Product>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Product> { product });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockProductsCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            var result = await _productRepository.GetProductIdBySKUAsync("P134");

            Assert.Equal("FRE123", result);
        }

        [Fact]
        public async Task GetProductIdByDocumentNumberAsync_ThrowsDataAccessException_OnError()
        {
            var product = new Product { _id = "FRE123", SKU = "P134", Name = "Playstation 5", Price = 108000 };

            var mockAsyncCursor = new Mock<IAsyncCursor<Product>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Product> { product });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockProductsCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<DataAccessException>(() => _productRepository.GetProductIdBySKUAsync("P134"));
        }

        [Fact]
        public async Task AddProductAsync_ReturnsId_WhenSuccessful()
        {
            var product = new Product { _id = "FRE123", SKU = "P134", Name = "Playstation 5", Price = 108000 };

            _mockProductsCollection.Setup(x => x.InsertOneAsync(product, null, default))
                           .Returns(Task.CompletedTask);

            var result = await _productRepository.AddProductAsync(product);

            Assert.Equal("FRE123", result);
        }

        [Fact]
        public async Task AddProductAsync_ThrowsDataAccessException_OnError()
        {
            var product = new Product { _id = "FRE123", SKU = "P134", Name = "Playstation 5", Price = 108000 };

            _mockProductsCollection.Setup(x => x.InsertOneAsync(product, null, default))
                           .Throws(new Exception("Data access exception."));

            await Assert.ThrowsAsync<DataAccessException>(() => _productRepository.AddProductAsync(product));
        }
    }
}
