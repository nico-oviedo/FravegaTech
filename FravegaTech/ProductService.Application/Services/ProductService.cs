using ProductService.Data.Repositories;
using ProductService.Domain;
using SharedKernel.Dtos;

namespace ProductService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        /// <inheritdoc/>
        public async Task<Product?> GetProductByIdAsync(string productId)
        {
            return await _productRepository.GetProductByIdAsync(productId);
        }

        /// <inheritdoc/>
        public async Task<string?> GetOrInsertNewProductAsync(ProductDto productDto)
        {
            string? productId = await _productRepository.GetProductIdBySKUAsync(productDto.SKU);

            if (productId is null)
            {
                //TODO: **Quizas agregue un mapeo automatico luego**
                Product product = new Product()
                {
                    SKU = productDto.SKU,
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price
                };

                productId = await _productRepository.InsertProductAsync(product);
            }

            return productId;
        }
    }
}
