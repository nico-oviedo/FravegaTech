using SharedKernel.Dtos;

namespace ProductService.Application.Services
{
    public interface IProductService
    {
        /// <summary>
        /// Gets product by id
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>Product dto object.</returns>
        Task<ProductDto> GetProductByIdAsync(string productId);

        /// <summary>
        /// Gets product id or insert new product
        /// </summary>
        /// <param name="productDto">Product dto object.</param>
        /// <returns>Product id.</returns>
        Task<string?> GetProductIdOrInsertNewProductAsync(ProductDto productDto);
    }
}
