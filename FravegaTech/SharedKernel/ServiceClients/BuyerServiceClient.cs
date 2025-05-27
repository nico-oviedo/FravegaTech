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
        /// Gets buyer id by document number from Buyer Service API
        /// </summary>
        /// <param name="documentNumber">Buer document number.</param>
        /// <returns>Buyer id.</returns>
        public async Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber)
        {
            return await GetAsync<string?>($"{_baseUrl}documentnumber/{documentNumber}");
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
