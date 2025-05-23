using ProductService.Domain;

namespace ProductService.Data.Repositories
{
    public interface IProductRepository
    {
        /// <summary>
        /// Gets product id by SKU
        /// </summary>
        /// <param name="sku">SKU.</param>
        /// <returns>Product id.</returns>
        Task<string?> GetProductIdBySKU(string sku);

        /// <summary>
        /// Inserts new product
        /// </summary>
        /// <param name="product">Product object.</param>
        /// <returns>Inserted product id.</returns>
        Task<string?> InsertProduct(Product product);
    }
}
