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
        /// <param name="sku">SKU.</param>
        /// <returns>Product id.</returns>
        Task<string?> GetProductIdBySKUAsync(string sku);

        /// <summary>
        /// Inserts new product
        /// </summary>
        /// <param name="product">Product object.</param>
        /// <returns>Inserted product id.</returns>
        Task<string?> InsertProductAsync(Product product);
    }
}
