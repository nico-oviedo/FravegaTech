using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Services;
using OrderService.Data.Repositories;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Dtos;

namespace OrderService.Application.Tests.Services
{
    public class EventValidationServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ILogger<EventValidationService>> _mockLogger;
        private readonly EventValidationService _eventValidationService;

        public EventValidationServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<EventValidationService>>();
            _eventValidationService = new EventValidationService(_mockOrderRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EventValidationService(null!, _mockLogger.Object));
            Assert.Throws<ArgumentNullException>(() => new EventValidationService(_mockOrderRepository.Object, null!));
        }

        [Fact]
        public void CreateNewOrderEvent_ShouldReturnCreatedEvent()
        {
            var result = _eventValidationService.CreateNewOrderEvent();

            Assert.NotNull(result);
            Assert.Equal("event-001", result.EventId);
            Assert.Equal(OrderStatus.Created, result.Type);
            Assert.Equal("System", result.User);
        }

        [Fact]
        public void CreateEventAddedDto_ShouldReturnEventAddedDto()
        {
            int orderId = 99;
            string previousStatus = "Created";
            string newStatus = "PaymentReceived";

            var dto = _eventValidationService.CreateEventAddedDto(orderId, previousStatus, newStatus);

            Assert.Equal(orderId, dto.OrderId);
            Assert.Equal(previousStatus, dto.PreviousStatus);
            Assert.Equal(newStatus, dto.NewStatus);
            Assert.True(dto.UpdatedOn <= DateTime.UtcNow);
        }

        [Fact]
        public void IsEventValidAndNotProcessedAsync_ShouldReturnTrue_WhenValidAndNotProcessed()
        {
            var order = new Order
            {
                Status = OrderStatus.Created,
                Events = new List<Event>()
            };

            var eventDto = new EventDto
            {
                Id = "event-002",
                Type = "PaymentReceived"
            };

            var (isValidAndUnique, notProcessed) = _eventValidationService.IsEventValidAndNotProcessedAsync(order, eventDto);

            Assert.True(isValidAndUnique);
            Assert.True(notProcessed);
        }

        [Fact]
        public void IsEventValidAndNotProcessedAsync_ShouldReturnFalse_WhenEventIdAlreadyExists()
        {
            var order = new Order
            {
                Status = OrderStatus.Created,
                Events = new List<Event>
            {
                new Event { EventId = "event-002", Type = OrderStatus.PaymentReceived }
            }
            };

            var eventDto = new EventDto
            {
                Id = "event-002",
                Type = "PaymentReceived"
            };

            var (isValidAndUnique, notProcessed) = _eventValidationService.IsEventValidAndNotProcessedAsync(order, eventDto);

            Assert.False(isValidAndUnique);
            Assert.False(notProcessed);
        }

        [Fact]
        public void IsEventValidAndNotProcessedAsync_ShouldThrow_WhenEventTypeInvalid()
        {
            var order = new Order
            {
                Status = OrderStatus.Created,
                Events = new List<Event>()
            };

            var eventDto = new EventDto
            {
                Id = "event-002",
                Type = "InvalidStatus"
            };

            Assert.Throws<Exception>(() => _eventValidationService.IsEventValidAndNotProcessedAsync(order, eventDto));
        }
    }
}
