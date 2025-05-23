using ProductService.Domain;
using SharedKernel.Dtos;

namespace ProductService.Application.Services
{
    public interface IProductService
    {
        /// <summary>
        /// Gets product by id
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>Product object.</returns>
        Task<Product?> GetProductByIdAsync(string productId);

        /// <summary>
        /// Gets or insert new product
        /// </summary>
        /// <param name="productDto">Product dto object.</param>
        /// <returns>Product id.</returns>
        Task<string?> GetOrInsertNewProductAsync(ProductDto productDto);
    }
}
