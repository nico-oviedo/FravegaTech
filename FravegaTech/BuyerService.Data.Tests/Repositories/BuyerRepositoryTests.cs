using BuyerService.Data.Repositories;
using BuyerService.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using SharedKernel.Exceptions;

namespace BuyerService.Data.Tests.Repositories
{
    public class BuyerRepositoryTests
    {
        private readonly Mock<ILogger<BuyerRepository>> _mockLogger = new();
        private readonly Mock<IMongoCollection<Buyer>> _mockBuyersCollection = new();
        private readonly BuyerRepository _buyerRepository;

        public BuyerRepositoryTests()
        {
            var configValues = new Dictionary<string, string?>
            {
                {"ConnectionStrings:MongoDB", "mongodb://localhost:27017"},
                {"ConnectionStrings:BuyerDatabase", "buyerDb"},
                {"ConnectionStrings:BuyersCollection", "buyers"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _buyerRepository = new BuyerRepository(configuration, _mockLogger.Object);

            typeof(BuyerRepository)
                .GetField("_buyers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(_buyerRepository, _mockBuyersCollection.Object);
        }

        [Fact]
        public async Task GetBuyerByIdAsync_ReturnsBuyer_WhenFound()
        {
            var buyer = new Buyer { _id = "FRE123", FirstName = "Rodrigo", LastName = "Hernandez", DocumentNumber = "57.899.332" };

            var mockAsyncCursor = new Mock<IAsyncCursor<Buyer>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Buyer> { buyer });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockBuyersCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Buyer>>(), It.IsAny<FindOptions<Buyer, Buyer>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            var result = await _buyerRepository.GetBuyerByIdAsync("FRE123");

            Assert.Equal(buyer, result);
        }

        [Fact]
        public async Task GetBuyerByIdAsync_ThrowsDataAccessException_OnError()
        {
            var buyer = new Buyer { _id = "FRE123", FirstName = "Rodrigo", LastName = "Hernandez", DocumentNumber = "57.899.332" };

            var mockAsyncCursor = new Mock<IAsyncCursor<Buyer>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Buyer> { buyer });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockBuyersCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Buyer>>(), It.IsAny<FindOptions<Buyer, Buyer>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Data access exception."));

            await Assert.ThrowsAsync<DataAccessException>(() => _buyerRepository.GetBuyerByIdAsync("FRE123"));

        }

        [Fact]
        public async Task GetBuyerIdByDocumentNumberAsync_ReturnsId_WhenBuyerExists()
        {
            var buyer = new Buyer { _id = "FRE123", FirstName = "Rodrigo", LastName = "Hernandez", DocumentNumber = "57.899.332" };

            var mockAsyncCursor = new Mock<IAsyncCursor<Buyer>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Buyer> { buyer });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockBuyersCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Buyer>>(), It.IsAny<FindOptions<Buyer, Buyer>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            var result = await _buyerRepository.GetBuyerIdByDocumentNumberAsync("57.899.332");

            Assert.Equal("FRE123", result);
        }

        [Fact]
        public async Task AddBuyerAsync_ReturnsId_WhenSuccessful()
        {
            var buyer = new Buyer { _id = "FRE123", FirstName = "Rodrigo", LastName = "Hernandez", DocumentNumber = "57.899.332" };

            _mockBuyersCollection.Setup(x => x.InsertOneAsync(buyer, null, default))
                           .Returns(Task.CompletedTask);

            var result = await _buyerRepository.AddBuyerAsync(buyer);

            Assert.Equal("FRE123", result);
        }

        [Fact]
        public async Task AddBuyerAsync_ThrowsDataAccessException_OnError()
        {
            var buyer = new Buyer { _id = "FRE123", FirstName = "Rodrigo", LastName = "Hernandez", DocumentNumber = "57.899.332" };

            _mockBuyersCollection.Setup(x => x.InsertOneAsync(buyer, null, default))
                           .Throws(new Exception("Data access exception."));

            await Assert.ThrowsAsync<DataAccessException>(() => _buyerRepository.AddBuyerAsync(buyer));
        }
    }
}
