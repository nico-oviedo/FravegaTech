using CounterService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using CounterServ = CounterService.Services;

namespace CounterService.Tests.Services
{
    public class CounterServiceTests
    {
        private readonly Mock<IMongoCollection<Counter>> _mockCounterCollection = new();
        private readonly Mock<ILogger<CounterServ.CounterService>> _mockLogger = new();
        private readonly CounterServ.CounterService _counterService;

        public CounterServiceTests()
        {
            var configValues = new Dictionary<string, string?>
            {
                {"ConnectionStrings:MongoDB", "mongodb://localhost:27017"},
                {"ConnectionStrings:CounterDatabase", "counterDb"},
                {"ConnectionStrings:CountersCollection", "counters"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _counterService = new CounterServ.CounterService(configuration, _mockLogger.Object);

            typeof(CounterServ.CounterService)
                .GetField("_counters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(_counterService, _mockCounterCollection.Object);
        }

        [Fact]
        public async Task GetNextSequenceValueAsync_ReturnsUpdatedSequenceValue()
        {
            var sequenceName = "test";
            var expectedValue = 42;
            var counter = new Counter { SequenceName = sequenceName, SequenceValue = expectedValue };

            var mockAsyncCursor = new Mock<IAsyncCursor<Counter>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns([counter]);
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

            _mockCounterCollection.Setup(c => c.FindOneAndUpdateAsync(It.IsAny<FilterDefinition<Counter>>(), It.IsAny<UpdateDefinition<Counter>>(),
                    It.IsAny<FindOneAndUpdateOptions<Counter>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(counter);

            var result = await _counterService.GetNextSequenceValueAsync(sequenceName);

            Assert.Equal(expectedValue, result);
        }
    }
}
