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
        /// Adds new product
        /// </summary>
        /// <param name="productDto">Product dto object.</param>
        /// <returns>Added product id.</returns>
        Task<string?> AddProductAsync(ProductDto productDto);
    }
}
