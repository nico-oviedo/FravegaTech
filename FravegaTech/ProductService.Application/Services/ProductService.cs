using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductService.Data.Repositories;
using ProductService.Domain;
using SharedKernel.Dtos;
using SharedKernel.Exceptions;

namespace ProductService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IMapper mapper, ILogger<ProductService> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ProductDto> GetProductByIdAsync(string productId)
        {
            try
            {
                _logger.LogInformation($"Trying to get Product with id: {productId}.");
                Product product = await _productRepository.GetProductByIdAsync(productId);

                if (product is null)
                {
                    _logger.LogError($"Product with id {productId} was not found.");
                    throw new NotFoundException(nameof(product), $"{GetType().Name}:{nameof(GetProductByIdAsync)}");
                }

                ProductDto productDto = _mapper.Map<ProductDto>(product);
                _logger.LogInformation($"Successfully get Product with id: {productId}.");
                return productDto;
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, $"Failed to map Product to ProductDto. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task<string> AddProductAsync(ProductDto productDto)
        {
            try
            {
                _logger.LogInformation("Trying to add Product.");
                string? productId = await _productRepository.GetProductIdBySKUAsync(productDto.SKU);

                if (productId is not null)
                {
                    _logger.LogInformation($"Product with SKU: {productDto.SKU} already exists. Returning ProductId: {productId}.");
                    return productId;
                }

                Product product = _mapper.Map<Product>(productDto);
                productId = await _productRepository.AddProductAsync(product);

                _logger.LogInformation($"Product added successfully. Returning ProductId: {productId}.");
                return productId;
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, $"Failed to map ProductDto to Product. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
    }
}
