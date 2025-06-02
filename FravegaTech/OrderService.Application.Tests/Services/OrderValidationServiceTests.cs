using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Services;
using OrderService.Data.Repositories;
using OrderService.Domain.Enums;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;

namespace OrderService.Application.Tests.Services
{
    public class OrderValidationServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ILogger<OrderValidationService>> _mockLogger;
        private readonly OrderValidationService _orderValidationService;

        public OrderValidationServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrderValidationService>>();
            _orderValidationService = new OrderValidationService(_mockOrderRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderValidationService(null!, _mockLogger.Object));
            Assert.Throws<ArgumentNullException>(() => new OrderValidationService(_mockOrderRepository.Object, null!));
        }

        [Fact]
        public async Task IsOrderValidAsync_ReturnsTrue_WhenReferenceIsUnique_AndTotalIsCorrect()
        {
            var orderRequest = GetValidOrderRequestDto();
            _mockOrderRepository.Setup(repo => repo.IsUniqueExternalReferenceInChannelAsync(orderRequest.ExternalReferenceId, SourceChannel.CallCenter))
                .ReturnsAsync(true);

            var result = await _orderValidationService.IsOrderValidAsync(orderRequest);

            Assert.True(result);
        }

        [Fact]
        public async Task IsOrderValidAsync_ReturnsFalse_WhenReferenceIsNotUnique()
        {
            var orderRequest = GetValidOrderRequestDto();
            _mockOrderRepository.Setup(repo => repo.IsUniqueExternalReferenceInChannelAsync(orderRequest.ExternalReferenceId, SourceChannel.CallCenter))
                .ReturnsAsync(false);

            var result = await _orderValidationService.IsOrderValidAsync(orderRequest);

            Assert.False(result);
        }

        [Fact]
        public async Task IsOrderValidAsync_ReturnsFalse_WhenTotalValueIsIncorrect()
        {
            var orderRequest = GetValidOrderRequestDto();
            orderRequest.TotalValue = 999;

            _mockOrderRepository.Setup(repo => repo.IsUniqueExternalReferenceInChannelAsync(orderRequest.ExternalReferenceId, SourceChannel.CallCenter))
                .ReturnsAsync(true);

            var result = await _orderValidationService.IsOrderValidAsync(orderRequest);

            Assert.False(result);
        }

        [Fact]
        public async Task IsOrderValidAsync_ThrowsException_WhenRepositoryFails()
        {
            var orderRequest = GetValidOrderRequestDto();
            _mockOrderRepository.Setup(repo => repo.IsUniqueExternalReferenceInChannelAsync(It.IsAny<string>(), It.IsAny<SourceChannel>()))
                .ThrowsAsync(new Exception("Data access error."));

            var ex = await Assert.ThrowsAsync<Exception>(() => _orderValidationService.IsOrderValidAsync(orderRequest));
            Assert.Equal("Data access error.", ex.Message);
        }

        private OrderRequestDto GetValidOrderRequestDto()
        {
            return new OrderRequestDto
            {
                Channel = "CallCenter",
                ExternalReferenceId = "ext-123",
                TotalValue = 200,
                Products = new List<OrderProductDto>
                {
                    new OrderProductDto { Price = 100, Quantity = 1 },
                    new OrderProductDto { Price = 50, Quantity = 2 }
                }
            };
        }
    }
}
