using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ProductService.Domain;

namespace ProductService.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;

        public ProductRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config.GetConnectionString("ProductDatabase"));
            _products = database.GetCollection<Product>(config.GetConnectionString("ProductsCollection"));
        }

        /// <inheritdoc/>
        public async Task<Product?> GetProductByIdAsync(string productId)
        {
            try
            {
                return await _products.Find(p => p._id == productId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                //Loguear exception
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<string?> GetProductIdBySKUAsync(string sku)
        {
            try
            {
                Product product = await _products.Find(p => p.SKU == sku).FirstOrDefaultAsync();
                return product?._id;
            }
            catch (Exception ex)
            {
                //Loguear exception
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<string?> InsertProductAsync(Product product)
        {
            try
            {
                await _products.InsertOneAsync(product);
                return product._id;
            }
            catch (Exception ex)
            {
                //Loguear exception
                return null;
            }
        }
    }
}
