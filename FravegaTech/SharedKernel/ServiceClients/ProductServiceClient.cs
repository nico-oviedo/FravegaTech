using Microsoft.Extensions.Logging;
using SharedKernel.Dtos;

namespace SharedKernel.ServiceClients
{
    public class ProductServiceClient : HttpServiceClient
    {
        private readonly string _baseUrl;
        private readonly ILogger<ProductServiceClient> _logger;

        public ProductServiceClient(HttpClient http, ILogger<ProductServiceClient> logger) : base(http, logger)
        {
            _baseUrl = http.BaseAddress?.OriginalString ?? throw new ArgumentNullException(nameof(http.BaseAddress));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets product from Product Service API
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>Product dto object.</returns>
        public async Task<ProductDto?> GetProductByIdAsync(string productId)
        {
            _logger.LogInformation($"Calling ProductService API - {nameof(GetProductByIdAsync)}.");
            return await GetAsync<ProductDto?>($"{_baseUrl}{productId}");
        }

        /// <summary>
        /// Sends product to Product Service API for addition
        /// </summary>
        /// <param name="productDto">Product dto object.</param>
        /// <returns>Added product id.</returns>
        public async Task<string?> AddProductAsync(ProductDto productDto)
        {
            _logger.LogInformation($"Calling ProductService API - {nameof(AddProductAsync)}.");
            return await PostAsync<ProductDto, string?>(_baseUrl, productDto);
        }
    }
}
