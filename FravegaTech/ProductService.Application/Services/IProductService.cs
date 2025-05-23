using Shared.Dtos;

namespace ProductService.Application.Services
{
    public interface IProductService
    {
        /// <summary>
        /// Gets or insert new product
        /// </summary>
        /// <param name="productDto">Product dto object.</param>
        /// <returns>Product id.</returns>
        Task<string?> GetOrInsertNewProduct(ProductDto productDto);
    }
}
