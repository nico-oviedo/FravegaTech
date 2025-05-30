using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ProductService.Domain;
using SharedKernel.Exceptions;

namespace ProductService.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(IConfiguration config, ILogger<ProductRepository> logger)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config.GetConnectionString("ProductDatabase"));
            _products = database.GetCollection<Product>(config.GetConnectionString("ProductsCollection"));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductByIdAsync(string productId)
        {
            try
            {
                return await _products.Find(p => p._id == productId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get Product by id {productId} from database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(GetProductByIdAsync)}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string?> GetProductIdBySKUAsync(string sku)
        {
            try
            {
                Product product = await _products.Find(p => p.SKU.ToLower() == sku.ToLower()).FirstOrDefaultAsync();
                return product?._id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get ProductId by SKU {sku} from database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(GetProductIdBySKUAsync)}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> AddProductAsync(Product product)
        {
            try
            {
                await _products.InsertOneAsync(product);
                return product._id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to insert new Product in database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(AddProductAsync)}", ex);
            }
        }
    }
}
