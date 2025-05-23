using BuyerService.Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BuyerService.Data.Repositories
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly IMongoCollection<Buyer> _buyers;

        public BuyerRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config.GetConnectionString("BuyerDatabase"));
            _buyers = database.GetCollection<Buyer>(config.GetConnectionString("BuyersCollection"));
        }

        /// <inheritdoc/>
        public async Task<Buyer?> GetBuyerByIdAsync(string buyerId)
        {
            try
            {
                return await _buyers.Find(b => b._id == buyerId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                //Loguear exception
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                Buyer buyer = await _buyers.Find(b => b.DocumentNumber == documentNumber).FirstOrDefaultAsync();
                return buyer?._id;
            }
            catch (Exception ex)
            {
                //Loguear exception
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<string?> InsertBuyerAsync(Buyer buyer)
        {
            try
            {
                await _buyers.InsertOneAsync(buyer);
                return buyer._id;
            }
            catch (Exception ex)
            {
                //Loguear exception
                return null;
            }
        }
    }
}
