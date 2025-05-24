using AutoMapper;
using ProductService.Data.Repositories;
using ProductService.Domain;
using SharedKernel.Dtos;

namespace ProductService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<ProductDto> GetProductByIdAsync(string productId)
        {
            Product product = await _productRepository.GetProductByIdAsync(productId);
            return _mapper.Map<ProductDto>(product);
        }

        /// <inheritdoc/>
        public async Task<string?> GetProductIdOrInsertNewProductAsync(ProductDto productDto)
        {
            string? productId = await _productRepository.GetProductIdBySKUAsync(productDto.SKU);

            if (productId is null)
            {
                Product product = _mapper.Map<Product>(productDto);
                productId = await _productRepository.InsertProductAsync(product);
            }

            return productId;
        }
    }
}
