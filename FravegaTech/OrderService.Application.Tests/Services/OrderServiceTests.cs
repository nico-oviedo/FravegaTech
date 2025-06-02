using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Services.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;
using SharedKernel.Exceptions;
using OrderApplication = OrderService.Application.Services;

namespace OrderService.Application.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository = new();
        private readonly Mock<IEventValidationService> _mockEventValidationService = new();
        private readonly Mock<IOrderValidationService> _mockOrderValidationService = new();
        private readonly Mock<IOrderExternalDataService> _mockOrderExternalDataService = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<ILogger<OrderApplication.OrderService>> _mockLogger = new();
        private readonly OrderApplication.OrderService _orderService;

        public OrderServiceTests()
        {
            _orderService = new OrderApplication.OrderService(_mockOrderRepository.Object, _mockEventValidationService.Object, _mockOrderValidationService.Object,
                _mockOrderExternalDataService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderApplication.OrderService(null!, _mockEventValidationService.Object,
                _mockOrderValidationService.Object, _mockOrderExternalDataService.Object, _mockMapper.Object, _mockLogger.Object));

            Assert.Throws<ArgumentNullException>(() => new OrderApplication.OrderService(_mockOrderRepository.Object, null!,
                _mockOrderValidationService.Object, _mockOrderExternalDataService.Object, _mockMapper.Object, _mockLogger.Object));

            Assert.Throws<ArgumentNullException>(() => new OrderApplication.OrderService(_mockOrderRepository.Object, _mockEventValidationService.Object,
                null!, _mockOrderExternalDataService.Object, _mockMapper.Object, _mockLogger.Object));

            Assert.Throws<ArgumentNullException>(() => new OrderApplication.OrderService(_mockOrderRepository.Object, _mockEventValidationService.Object,
                _mockOrderValidationService.Object, null!, _mockMapper.Object, _mockLogger.Object));

            Assert.Throws<ArgumentNullException>(() => new OrderApplication.OrderService(_mockOrderRepository.Object, _mockEventValidationService.Object,
                _mockOrderValidationService.Object, _mockOrderExternalDataService.Object, null!, _mockLogger.Object));

            Assert.Throws<ArgumentNullException>(() => new OrderApplication.OrderService(_mockOrderRepository.Object, _mockEventValidationService.Object,
                _mockOrderValidationService.Object, _mockOrderExternalDataService.Object, _mockMapper.Object, null!));
        }

        [Fact]
        public async Task GetFullOrderAsync_ReturnsMappedOrder_WhenOrderExists()
        {
            var order = new Order { OrderId = 1, BuyerId = "RYY567", Products = [] };
            var buyerDto = new BuyerDto();
            var productsDto = new List<OrderProductDto>();
            var orderTranslatedDto = new OrderTranslatedDto();

            _mockOrderRepository.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync(order);
            _mockOrderExternalDataService.Setup(e => e.GetBuyerDtoAndOrderProductsDtoFromOrderAsync(order)).ReturnsAsync((buyerDto, productsDto));
            _mockMapper.Setup(m => m.Map<OrderTranslatedDto>(order)).Returns(orderTranslatedDto);

            var result = await _orderService.GetFullOrderAsync(1);

            Assert.Equal(orderTranslatedDto, result);
        }

        [Fact]
        public async Task GetFullOrderAsync_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            _mockOrderRepository.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync((Order)null!);
            await Assert.ThrowsAsync<NotFoundException>(() => _orderService.GetFullOrderAsync(1));
        }

        [Fact]
        public async Task GetFullOrderAsync_ReturnsException_WhenAutoMapperMappingException()
        {
            var order = new Order { OrderId = 1, BuyerId = "RYY567", Products = [] };
            var buyerDto = new BuyerDto();
            var productsDto = new List<OrderProductDto>();
            var orderTranslatedDto = new OrderTranslatedDto();

            _mockOrderRepository.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync(order);
            _mockOrderExternalDataService.Setup(e => e.GetBuyerDtoAndOrderProductsDtoFromOrderAsync(order)).ReturnsAsync((buyerDto, productsDto));
            _mockMapper.Setup(m => m.Map<OrderTranslatedDto>(order)).Throws(new AutoMapperMappingException());

            await Assert.ThrowsAsync<Exception>(() => _orderService.GetFullOrderAsync(1));
        }

        [Fact]
        public async Task SearchOrdersAsync_ReturnsMappedOrders_WhenFound()
        {
            var order = new Order { OrderId = 1 };
            var orders = new List<Order> { order };
            var buyerDto = new BuyerDto();
            var productsDto = new List<OrderProductDto>();
            var orderDto = new OrderDto { Buyer = buyerDto, Products = productsDto, Events = [new()] };

            _mockOrderExternalDataService.Setup(e => e.GetBuyerIdByDocumentNumberAsync("ABC123")).ReturnsAsync(string.Empty);
            _mockMapper.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);

            _mockOrderRepository.Setup(r => r.SearchOrdersAsync(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<OrderStatus?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(orders);

            var result = await _orderService.SearchOrdersAsync(1, "ABC123", "PaymentReceived", null, null);

            Assert.Single(result);
            Assert.Equal(orderDto, result.First());
        }

        [Fact]
        public async Task SearchOrdersAsync_ReturnsNotFoundException()
        {
            var order = new Order { OrderId = 1 };
            var orders = new List<Order> { order };
            var buyerDto = new BuyerDto();
            var productsDto = new List<OrderProductDto>();
            var orderDto = new OrderDto { Buyer = buyerDto, Products = productsDto, Events = [new()] };

            _mockOrderExternalDataService.Setup(e => e.GetBuyerIdByDocumentNumberAsync("ABC123")).ReturnsAsync(string.Empty);
            _mockMapper.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);

            _mockOrderRepository.Setup(r => r.SearchOrdersAsync(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<OrderStatus?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync((List<Order>)null!);

            await Assert.ThrowsAsync<NotFoundException>(() => _orderService.SearchOrdersAsync(1, "ABC123", "PaymentReceived", null, null));
        }

        [Fact]
        public async Task SearchOrdersAsync_ReturnsException()
        {
            var order = new Order { OrderId = 1 };
            var orders = new List<Order> { order };
            var buyerDto = new BuyerDto();
            var productsDto = new List<OrderProductDto>();
            var orderDto = new OrderDto { Buyer = buyerDto, Products = productsDto, Events = [new()] };

            _mockOrderExternalDataService.Setup(e => e.GetBuyerIdByDocumentNumberAsync("ABC123")).ReturnsAsync(string.Empty);
            _mockMapper.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);

            _mockOrderRepository.Setup(r => r.SearchOrdersAsync(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<OrderStatus?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ThrowsAsync(new AutoMapperMappingException());

            await Assert.ThrowsAsync<Exception>(() => _orderService.SearchOrdersAsync(1, "ABC123", "PaymentReceived", null, null));
        }

        [Fact]
        public async Task AddOrderAsync_ReturnsOrderCreatedDto_WhenOrderIsValid()
        {
            var orderRequestDto = new OrderRequestDto { Products = new(), Buyer = new() };
            var order = new Order();
            var orderCreatedDto = new OrderCreatedDto();

            _mockOrderValidationService.Setup(v => v.IsOrderValidAsync(orderRequestDto)).ReturnsAsync(true);
            _mockMapper.Setup(m => m.Map<Order>(orderRequestDto)).Returns(order);
            _mockOrderExternalDataService.Setup(e => e.GetDataFromOrderRequestDtoAsync(orderRequestDto)).ReturnsAsync((1, "RYY567", new List<OrderProduct>()));
            _mockEventValidationService.Setup(e => e.CreateNewOrderEvent()).Returns(new Event());
            _mockOrderRepository.Setup(r => r.AddOrderAsync(It.IsAny<Order>())).ReturnsAsync("1");
            _mockMapper.Setup(m => m.Map<OrderCreatedDto>(It.IsAny<Order>())).Returns(orderCreatedDto);

            var result = await _orderService.AddOrderAsync(orderRequestDto);

            Assert.Equal(orderCreatedDto, result);
        }

        [Fact]
        public async Task AddOrderAsync_ThrowsBusinessValidationException()
        {
            var orderRequestDto = new OrderRequestDto { Products = new(), Buyer = new() };
            _mockOrderValidationService.Setup(v => v.IsOrderValidAsync(orderRequestDto)).ReturnsAsync(false);

            await Assert.ThrowsAsync<BusinessValidationException>(() => _orderService.AddOrderAsync(orderRequestDto));
        }

        [Fact]
        public async Task AddOrderAsync_ThrowsException()
        {
            var orderRequestDto = new OrderRequestDto { Products = new(), Buyer = new() };
            var order = new Order();
            var orderCreatedDto = new OrderCreatedDto();

            _mockOrderValidationService.Setup(v => v.IsOrderValidAsync(orderRequestDto)).ReturnsAsync(true);
            _mockMapper.Setup(m => m.Map<Order>(orderRequestDto)).Returns(order);
            _mockOrderExternalDataService.Setup(e => e.GetDataFromOrderRequestDtoAsync(orderRequestDto)).ReturnsAsync((1, "RYY567", new List<OrderProduct>()));
            _mockEventValidationService.Setup(e => e.CreateNewOrderEvent()).Returns(new Event());
            _mockOrderRepository.Setup(r => r.AddOrderAsync(It.IsAny<Order>())).ReturnsAsync("1");
            _mockMapper.Setup(m => m.Map<OrderCreatedDto>(It.IsAny<Order>())).Throws(new AutoMapperMappingException());

            await Assert.ThrowsAsync<Exception>(() => _orderService.AddOrderAsync(orderRequestDto));
        }

        [Fact]
        public async Task AddEventToOrderAsync_ReturnsEventAddedDto_WhenValid()
        {
            var order = new Order { OrderId = 1, Status = OrderStatus.Returned };
            var eventDto = new EventDto { Type = "Returned" };
            var eventAddedDto = new EventAddedDto();

            _mockOrderRepository.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync(order);
            _mockEventValidationService.Setup(e => e.IsEventValidAndNotProcessedAsync(order, eventDto)).Returns((true, true));
            _mockMapper.Setup(m => m.Map<Event>(eventDto)).Returns(new Event { Type = OrderStatus.Returned });
            _mockOrderRepository.Setup(r => r.AddEventAsync(1, It.IsAny<Event>())).ReturnsAsync(true);
            _mockOrderRepository.Setup(r => r.UpdateOrderStatusAsync(1, OrderStatus.Returned)).ReturnsAsync(true);
            _mockEventValidationService.Setup(e => e.CreateEventAddedDto(1, order.Status.ToString(), OrderStatus.Returned.ToString()))
                .Returns(eventAddedDto);

            var result = await _orderService.AddEventToOrderAsync(1, eventDto);

            Assert.Equal(eventAddedDto, result);
        }

        [Fact]
        public async Task AddEventToOrderAsync_ThrowsNotFoundException_WhenOrderNotFound()
        {
            var eventDto = new EventDto { Type = "Returned" };
            _mockOrderRepository.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync((Order)null!);

            await Assert.ThrowsAsync<NotFoundException>(() => _orderService.AddEventToOrderAsync(1, eventDto));
        }

        [Fact]
        public async Task AddEventToOrderAsync_ThrowsBusinessValidationException_WhenEventInvalid()
        {
            var order = new Order { OrderId = 1, Status = OrderStatus.Returned };
            var eventDto = new EventDto { Type = "Returned" };

            _mockOrderRepository.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync(order);
            _mockEventValidationService.Setup(e => e.IsEventValidAndNotProcessedAsync(order, eventDto)).Returns((false, true));

            await Assert.ThrowsAsync<BusinessValidationException>(() => _orderService.AddEventToOrderAsync(1, eventDto));
        }

        [Fact]
        public async Task AddEventToOrderAsync_ReturnsEventAddedDto_WhenEventAlreadyProcessed()
        {
            var order = new Order { OrderId = 1, Status = OrderStatus.Returned };
            var eventDto = new EventDto { Type = "Returned" };
            var eventAddedDto = new EventAddedDto();

            _mockOrderRepository.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync(order);
            _mockEventValidationService.Setup(e => e.IsEventValidAndNotProcessedAsync(order, eventDto)).Returns((true, false));
            _mockEventValidationService.Setup(e => e.CreateEventAddedDto(1, order.Status.ToString(), OrderStatus.Returned.ToString()))
                .Returns(eventAddedDto);

            var result = await _orderService.AddEventToOrderAsync(1, eventDto);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddEventToOrderAsync_ThrowsException()
        {
            var order = new Order { OrderId = 1, Status = OrderStatus.Returned };
            var eventDto = new EventDto { Type = "Returned" };

            _mockOrderRepository.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync(order);
            _mockEventValidationService.Setup(e => e.IsEventValidAndNotProcessedAsync(order, eventDto)).Returns((true, true));
            _mockMapper.Setup(m => m.Map<Event>(eventDto)).Throws(new AutoMapperMappingException());

            await Assert.ThrowsAsync<Exception>(() => _orderService.AddEventToOrderAsync(1, eventDto));
        }
    }
}
