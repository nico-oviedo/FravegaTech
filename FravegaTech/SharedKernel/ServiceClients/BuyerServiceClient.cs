using System.Net.Http.Json;
using SharedKernel.Dtos;

namespace SharedKernel.ServiceClients
{
    public class BuyerServiceClient
    {
        private readonly HttpClient _http;

        public BuyerServiceClient(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        /// <summary>
        /// Gets buyer dto from Buyer Service API
        /// </summary>
        /// <param name="buyerId">Buyer id.</param>
        /// <returns>Buyer dto object.</returns>
        public async Task<BuyerDto?> GetBuyerByIdAsync(string buyerId)
        {
            try
            {
                var response = await _http.GetAsync($"{_http.BaseAddress}{buyerId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<BuyerDto>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets buyer id by document number from Buyer Service API
        /// </summary>
        /// <param name="documentNumber">Buer document number.</param>
        /// <returns>Buyer id.</returns>
        public async Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                var response = await _http.GetAsync($"{_http.BaseAddress}documentnumber/{documentNumber}");

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
        /// Sends buyer to Buyer Service API for addition
        /// </summary>
        /// <param name="buyerDto">Buyer dto object.</param>
        /// <returns>Added buyer id.</returns>
        public async Task<string?> AddBuyerAsync(BuyerDto buyerDto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(_http.BaseAddress, buyerDto);

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
