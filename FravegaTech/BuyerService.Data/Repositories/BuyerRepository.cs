using BuyerService.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SharedKernel.Exceptions;

namespace BuyerService.Data.Repositories
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly IMongoCollection<Buyer> _buyers;
        private readonly ILogger<BuyerRepository> _logger;

        public BuyerRepository(IConfiguration config, ILogger<BuyerRepository> logger)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config.GetConnectionString("BuyerDatabase"));
            _buyers = database.GetCollection<Buyer>(config.GetConnectionString("BuyersCollection"));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<Buyer> GetBuyerByIdAsync(string buyerId)
        {
            try
            {
                return await _buyers.Find(b => b._id == buyerId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get Buyer by id from database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(GetBuyerByIdAsync)}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                Buyer buyer = await _buyers.Find(b => b.DocumentNumber.ToLower() == documentNumber.ToLower()).FirstOrDefaultAsync();
                return buyer?._id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get BuyerId by document number from database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(GetBuyerIdByDocumentNumberAsync)}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> AddBuyerAsync(Buyer buyer)
        {
            try
            {
                await _buyers.InsertOneAsync(buyer);
                return buyer._id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to insert new Buyer in database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(AddBuyerAsync)}", ex);
            }
        }
    }
}
