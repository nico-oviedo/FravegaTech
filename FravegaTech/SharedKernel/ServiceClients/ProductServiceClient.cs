using System.Net.Http.Json;
using SharedKernel.Dtos;

namespace SharedKernel.ServiceClients
{
    public class ProductServiceClient
    {
        private readonly HttpClient _http;

        public ProductServiceClient(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        /// <summary>
        /// Gets product from Product Service API
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>Product dto object.</returns>
        public async Task<ProductDto?> GetProductByIdAsync(string productId)
        {
            try
            {
                var response = await _http.GetAsync($"{_http.BaseAddress}{productId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets product id by SKU from Product Service API
        /// </summary>
        /// <param name="sku">Product SKU.</param>
        /// <returns>Product id.</returns>
        public async Task<string?> GetProductIdBySKUAsync(string sku)
        {
            try
            {
                var response = await _http.GetAsync($"{_http.BaseAddress}sku/{sku}");

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<string>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Sends product to Product Service API for addition
        /// </summary>
        /// <param name="productDto">Product dto object.</param>
        /// <returns>Added product id.</returns>
        public async Task<string?> AddProductAsync(ProductDto productDto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(_http.BaseAddress, productDto);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<string>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
