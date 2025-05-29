using ProductService.Domain;

namespace ProductService.Data.Repositories
{
    public interface IProductRepository
    {
        /// <summary>
        /// Gets product by id
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>Product object.</returns>
        Task<Product> GetProductByIdAsync(string productId);

        /// <summary>
        /// Gets product id by SKU
        /// </summary>
        /// <param name="sku">Product SKU.</param>
        /// <returns>Product id.</returns>
        Task<string?> GetProductIdBySKUAsync(string sku);

        /// <summary>
        /// Adds new product
        /// </summary>
        /// <param name="product">Product object.</param>
        /// <returns>Inserted product id.</returns>
        Task<string> AddProductAsync(Product product);
    }
}
