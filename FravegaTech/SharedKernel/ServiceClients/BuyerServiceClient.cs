using SharedKernel.Dtos;

namespace SharedKernel.ServiceClients
{
    public class BuyerServiceClient : HttpServiceClient
    {
        private readonly string _baseUrl;

        public BuyerServiceClient(HttpClient http) : base(http)
        {
            _baseUrl = http.BaseAddress?.OriginalString ?? throw new ArgumentNullException(nameof(http.BaseAddress));
        }

        /// <summary>
        /// Gets buyer dto from Buyer Service API
        /// </summary>
        /// <param name="buyerId">Buyer id.</param>
        /// <returns>Buyer dto object.</returns>
        public async Task<BuyerDto?> GetBuyerByIdAsync(string buyerId)
        {
            return await GetAsync<BuyerDto?>($"{_baseUrl}{buyerId}");
        }

        /// <summary>
        /// Sends buyer to Buyer Service API for addition
        /// </summary>
        /// <param name="buyerDto">Buyer dto object.</param>
        /// <returns>Added buyer id.</returns>
        public async Task<string?> AddBuyerAsync(BuyerDto buyerDto)
        {
            return await PostAsync<BuyerDto, string?>(_baseUrl, buyerDto);
        }
    }
}
