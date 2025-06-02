using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using OrderService.Data.Repositories;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Exceptions;
using System.Linq.Expressions;

namespace OrderService.Data.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        private readonly Mock<ILogger<OrderRepository>> _mockLogger = new();
        private readonly Mock<IMongoCollection<Order>> _mockOrdersCollection = new();
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryTests()
        {
            var configValues = new Dictionary<string, string?>
            {
                {"ConnectionStrings:MongoDB", "mongodb://localhost:27017"},
                {"ConnectionStrings:OrderDatabase", "orderDb"},
                {"ConnectionStrings:OrdersCollection", "orders"},
                {"TimeZones:TimeZoneARG", "America/Argentina/Buenos_Aires"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _orderRepository = new OrderRepository(configuration, _mockLogger.Object);

            typeof(OrderRepository)
                .GetField("_orders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(_orderRepository, _mockOrdersCollection.Object);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOrder_WhenFound()
        {
            var order = new Order { _id = "FRE123", OrderId = 14, ExternalReferenceId = "ZZZ454", Status = OrderStatus.Created };

            var mockAsyncCursor = new Mock<IAsyncCursor<Order>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Order> { order });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockOrdersCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<FindOptions<Order, Order>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            var result = await _orderRepository.GetByOrderIdAsync(14);

            Assert.Equal(order, result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ThrowsDataAccessException_OnError()
        {
            var order = new Order { _id = "FRE123", OrderId = 14, ExternalReferenceId = "ZZZ454", Status = OrderStatus.Created };

            var mockAsyncCursor = new Mock<IAsyncCursor<Order>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Order> { order });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockOrdersCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<FindOptions<Order, Order>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Data access exception."));

            await Assert.ThrowsAsync<DataAccessException>(() => _orderRepository.GetByOrderIdAsync(14));
        }

        [Fact]
        public async Task IsUniqueExternalReferenceInChannelAsync_ReturnsFalse_WhenOrderExists()
        {
            var order = new Order { _id = "FRE123", OrderId = 14, ExternalReferenceId = "ZZZ454", Status = OrderStatus.Created };

            var mockAsyncCursor = new Mock<IAsyncCursor<Order>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Order> { order });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockOrdersCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<FindOptions<Order, Order>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            var result = await _orderRepository.IsUniqueExternalReferenceInChannelAsync("ZZZ454", SourceChannel.Ecommerce);

            Assert.False(result);
        }

        [Fact]
        public async Task IsUniqueExternalReferenceInChannelAsync_ThrowsDataAccessException_OnError()
        {
            var order = new Order { _id = "FRE123", OrderId = 14, ExternalReferenceId = "ZZZ454", Status = OrderStatus.Created };

            var mockAsyncCursor = new Mock<IAsyncCursor<Order>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(new List<Order> { order });
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockOrdersCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<FindOptions<Order, Order>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<DataAccessException>(() => _orderRepository.IsUniqueExternalReferenceInChannelAsync("ZZZ454", SourceChannel.Ecommerce));
        }

        [Fact]
        public async Task AddOrderAsync_ReturnsId_WhenSuccessful()
        {
            var order = new Order { _id = "FRE123", OrderId = 14, ExternalReferenceId = "ZZZ454", Status = OrderStatus.Created };

            _mockOrdersCollection.Setup(x => x.InsertOneAsync(order, null, default))
                           .Returns(Task.CompletedTask);

            var result = await _orderRepository.AddOrderAsync(order);

            Assert.Equal("FRE123", result);
        }

        [Fact]
        public async Task AddOrderAsync_ThrowsDataAccessException_OnError()
        {
            var order = new Order { _id = "FRE123", OrderId = 14, ExternalReferenceId = "ZZZ454", Status = OrderStatus.Created };

            _mockOrdersCollection.Setup(x => x.InsertOneAsync(order, null, default))
                           .Throws(new Exception("Data access exception."));

            await Assert.ThrowsAsync<DataAccessException>(() => _orderRepository.AddOrderAsync(order));
        }

        [Fact]
        public async Task AddEventAsync_ReturnsTrue_WhenSuccessful()
        {
            int orderId = 1;
            var newEvent = new Event { EventId = "test-event", Type = OrderStatus.PaymentReceived };
            var mockUpdateResult = new Mock<UpdateResult>();

            mockUpdateResult.SetupGet(r => r.MatchedCount).Returns(1);
            mockUpdateResult.SetupGet(r => r.ModifiedCount).Returns(1);

            _mockOrdersCollection.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<UpdateDefinition<Order>>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockUpdateResult.Object);

            var result = await _orderRepository.AddEventAsync(orderId, newEvent);

            Assert.True(result);
        }

        [Fact]
        public async Task AddEventAsync_ThrowsDataAccessException_OnError()
        {
            int orderId = 1;
            var newEvent = new Event { EventId = "test-event", Type = OrderStatus.PaymentReceived };
            var mockUpdateResult = new Mock<UpdateResult>();

            _mockOrdersCollection.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<UpdateDefinition<Order>>(), null, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Data access exception."));

            await Assert.ThrowsAsync<DataAccessException>(() => _orderRepository.AddEventAsync(orderId, newEvent));
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ReturnsTrue_WhenSuccessful()
        {
            int orderId = 1;
            var newStatus = OrderStatus.PaymentReceived;
            var mockUpdateResult = new Mock<UpdateResult>();

            mockUpdateResult.SetupGet(r => r.MatchedCount).Returns(1);
            mockUpdateResult.SetupGet(r => r.ModifiedCount).Returns(1);

            _mockOrdersCollection.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<UpdateDefinition<Order>>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockUpdateResult.Object);

            var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ThrowsDataAccessException_OnError()
        {
            int orderId = 1;
            var newStatus = OrderStatus.PaymentReceived;
            var mockUpdateResult = new Mock<UpdateResult>();

            _mockOrdersCollection.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<UpdateDefinition<Order>>(), null, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Data access exception."));

            await Assert.ThrowsAsync<DataAccessException>(() => _orderRepository.UpdateOrderStatusAsync(orderId, newStatus));
        }

        [Fact]
        public async Task SearchOrdersAsync_ReturnsOrders()
        {
            var orderList = new List<Order>
            {
                new Order { OrderId = 1, BuyerId = "B345FG", Status = OrderStatus.Created, PurchaseDate = DateTime.UtcNow }
            };

            var mockAsyncCursor = new Mock<IAsyncCursor<Order>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(orderList);
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockOrdersCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<FindOptions<Order, Order>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            var result = await _orderRepository.SearchOrdersAsync(1, null, null, null, null);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].OrderId);
        }

        [Fact]
        public async Task SearchOrdersAsync_ThrowsDataAccessException()
        {
            var orderList = new List<Order>
            {
                new Order { OrderId = 1, BuyerId = "B345FG", Status = OrderStatus.Created, PurchaseDate = DateTime.UtcNow }
            };

            var mockAsyncCursor = new Mock<IAsyncCursor<Order>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(orderList);
            mockAsyncCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockOrdersCollection.Setup(c =>
                    c.FindAsync(It.IsAny<FilterDefinition<Order>>(), It.IsAny<FindOptions<Order, Order>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Data access exception."));

            await Assert.ThrowsAsync<DataAccessException>(() => _orderRepository.SearchOrdersAsync(1, null, null, null, null));
        }
    }
}
