using CounterService.Models;
using MongoDB.Driver;

namespace CounterService.Services
{
    public class CounterService : ICounterService
    {
        private readonly IMongoCollection<Counter> _counters;

        public CounterService()
        {
            var client = new MongoClient("mongodb://localhost:27017"); //Obtener por archivo de configuracion
            var database = client.GetDatabase("counterDb");
            _counters = database.GetCollection<Counter>("counters");
        }

        /// <inheritdoc/>
        public async Task<int> GetNextSequenceValueAsync(string sequenceName)
        {
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
    }
}
