using CounterService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SharedKernel.Exceptions;

namespace CounterService.Services
{
    public class CounterService : ICounterService
    {
        private readonly IMongoCollection<Counter> _counters;
        private readonly ILogger<CounterService> _logger;

        public CounterService(IConfiguration config, ILogger<CounterService> logger)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config.GetConnectionString("CounterDatabase"));
            _counters = database.GetCollection<Counter>(config.GetConnectionString("CountersCollection"));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<int> GetNextSequenceValueAsync(string sequenceName)
        {
            try
            {
                _logger.LogInformation($"Getting next sequence value for {sequenceName}.");

                var filter = Builders<Counter>.Filter.Eq(c => c.SequenceName, sequenceName);
                var update = Builders<Counter>.Update.Inc(c => c.SequenceValue, 1);
                var options = new FindOneAndUpdateOptions<Counter>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                };

                var counter = await _counters.FindOneAndUpdateAsync(filter, update, options);
                return counter.SequenceValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get next sequence value for {sequenceName}. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(GetNextSequenceValueAsync)}", ex);
            }
        }
    }
}
