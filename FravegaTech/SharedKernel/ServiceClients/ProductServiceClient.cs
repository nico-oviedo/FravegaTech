using SharedKernel.Dtos;

namespace SharedKernel.ServiceClients
{
    public class ProductServiceClient : HttpServiceClient
    {
        private readonly string _baseUrl;

        public ProductServiceClient(HttpClient http) : base(http)
        {
            _baseUrl = http.BaseAddress?.OriginalString ?? throw new ArgumentNullException(nameof(http.BaseAddress));
        }

        /// <summary>
        /// Gets product from Product Service API
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>Product dto object.</returns>
        public async Task<ProductDto?> GetProductByIdAsync(string productId)
        {
            return await GetAsync<ProductDto?>($"{_baseUrl}{productId}");
        }

        /// <summary>
        /// Gets product id by SKU from Product Service API
        /// </summary>
        /// <param name="sku">Product SKU.</param>
        /// <returns>Product id.</returns>
        public async Task<string?> GetProductIdBySKUAsync(string sku)
        {
            return await GetAsync<string?>($"{_baseUrl}sku/{sku}");
        }

        /// <summary>
        /// Sends product to Product Service API for addition
        /// </summary>
        /// <param name="productDto">Product dto object.</param>
        /// <returns>Added product id.</returns>
        public async Task<string?> AddProductAsync(ProductDto productDto)
        {
            return await PostAsync<ProductDto, string?>(_baseUrl, productDto);
        }
    }
}
