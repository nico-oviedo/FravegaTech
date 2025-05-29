using Microsoft.Extensions.Logging;
using SharedKernel.Dtos;

namespace SharedKernel.ServiceClients
{
    public class BuyerServiceClient : HttpServiceClient
    {
        private readonly string _baseUrl;
        private readonly ILogger<BuyerServiceClient> _logger;

        public BuyerServiceClient(HttpClient http, ILogger<BuyerServiceClient> logger) : base(http, logger)
        {
            _baseUrl = http.BaseAddress?.OriginalString ?? throw new ArgumentNullException(nameof(http.BaseAddress));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets buyer dto from Buyer Service API
        /// </summary>
        /// <param name="buyerId">Buyer id.</param>
        /// <returns>Buyer dto object.</returns>
        public async Task<BuyerDto?> GetBuyerByIdAsync(string buyerId)
        {
            _logger.LogInformation($"Calling BuyerService API - {nameof(GetBuyerByIdAsync)}.");
            return await GetAsync<BuyerDto?>($"{_baseUrl}{buyerId}");
        }

        /// <summary>
        /// Gets buyer id by document number from Buyer Service API
        /// </summary>
        /// <param name="documentNumber">Buer document number.</param>
        /// <returns>Buyer id.</returns>
        public async Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber)
        {
            _logger.LogInformation($"Calling BuyerService API - {nameof(GetBuyerIdByDocumentNumberAsync)}.");
            return await GetAsync<string?>($"{_baseUrl}documentnumber/{documentNumber}");
        }

        /// <summary>
        /// Sends buyer to Buyer Service API for addition
        /// </summary>
        /// <param name="buyerDto">Buyer dto object.</param>
        /// <returns>Added buyer id.</returns>
        public async Task<string?> AddBuyerAsync(BuyerDto buyerDto)
        {
            _logger.LogInformation($"Calling BuyerService API - {nameof(AddBuyerAsync)}.");
            return await PostAsync<BuyerDto, string?>(_baseUrl, buyerDto);
        }
    }
}
