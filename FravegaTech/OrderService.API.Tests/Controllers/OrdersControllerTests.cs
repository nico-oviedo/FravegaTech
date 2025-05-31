using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.API.Controllers;
using OrderService.Application.Services.Interfaces;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;
using SharedKernel.Exceptions;

namespace OrderService.API.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<ILogger<OrdersController>> _mockLogger;
        private readonly OrdersController _ordersController;

        public OrdersControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _ordersController = new OrdersController(_mockOrderService.Object, _mockLogger.Object);
        }

        #region GetAsync

        [Fact]
        public async Task GetAsync_ReturnsOk_WhenOrderExists()
        {
            var orderId = 12;
            var orderTranslatedDto = new OrderTranslatedDto { OrderId = orderId, ExternalReferenceId = "ABC478", Status = "Created" };
            _mockOrderService.Setup(s => s.GetFullOrderAsync(orderId))
                .ReturnsAsync(orderTranslatedDto);

            var result = await _ordersController.GetAsync(orderId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orderTranslatedDto, okResult.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsBadRequest_WhenIdIsZero()
        {
            var result = await _ordersController.GetAsync(0);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id de la orden es requerido.", badRequest.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound_WhenOrderNotFound()
        {
            _mockOrderService.Setup(s => s.GetFullOrderAsync(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException("Order", "OrderService"));

            var result = await _ordersController.GetAsync(14);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Orden no fue encontrada.", notFound.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenUnhandledException()
        {
            _mockOrderService.Setup(s => s.GetFullOrderAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            var result = await _ordersController.GetAsync(17);
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Un error interno ha ocurrido.", serverError.Value);
        }

        #endregion

        #region SearchAsync

        [Fact]
        public async Task SearchAsync_ReturnsOk_WhenFoundOrders()
        {
            var orderId = 7;
            var documentNumber = "25.333.621";
            var orders = new List<OrderDto>() { new OrderDto() { OrderId = 7, Status = "Returned" } };

            _mockOrderService.Setup(s => s.SearchOrdersAsync(orderId, documentNumber, null, null, null))
                .ReturnsAsync(orders);

            var result = await _ordersController.SearchAsync(orderId, documentNumber, null, null, null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orders, okResult.Value);
        }

        [Fact]
        public async Task SearchAsync_ReturnsNotFound_WhenOrdersNotFound()
        {
            var orderId = 7;
            var status = "Created";

            _mockOrderService.Setup(s => s.SearchOrdersAsync(orderId, null, status, null, null))
                .ThrowsAsync(new NotFoundException("Orders", "OrderService"));

            var result = await _ordersController.SearchAsync(orderId, null, status, null, null);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No fueron encontradas Ordenes con los filtros ingresados.", notFoundResult.Value);
        }

        [Fact]
        public async Task SearchAsync_ReturnsInternalServerError_WhenUnhandledException()
        {
            var status = "Cancelled";
            var now = DateTime.Now;

            _mockOrderService.Setup(s => s.SearchOrdersAsync(null, null, status, now, now.AddDays(5)))
                .ThrowsAsync(new Exception());

            var result = await _ordersController.SearchAsync(null, null, status, now, now.AddDays(5));
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Un error interno ha ocurrido.", serverError.Value);
        }

        #endregion

        #region PostAsync

        [Fact]
        public async Task PostAsync_ReturnsOk_WhenOrderAdded()
        {
            var orderRequestDto = new OrderRequestDto { ExternalReferenceId = "ABC478", TotalValue = 3500 };
            var orderCreatedDto = new OrderCreatedDto { OrderId = 10, Status = "Created" };
            _mockOrderService.Setup(s => s.AddOrderAsync(orderRequestDto)).ReturnsAsync(orderCreatedDto);

            var result = await _ordersController.PostAsync(orderRequestDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orderCreatedDto, okResult.Value);
        }

        [Fact]
        public async Task PostAsync_Returns500_WhenServiceThrows()
        {
            var orderRequestDto = new OrderRequestDto { ExternalReferenceId = "ABC478", TotalValue = 3500 };
            _mockOrderService.Setup(s => s.AddOrderAsync(orderRequestDto)).ThrowsAsync(new DataAccessException("OrderService", new Exception()));

            var result = await _ordersController.PostAsync(orderRequestDto);
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Ocurrió un error al ingresar una nueva orden en el sistema.", serverError.Value);
        }

        #endregion

        #region AddEventAsync

        [Fact]
        public async Task AddEventAsync_ReturnsOk_WhenEventAdded()
        {
            var orderId = 10;
            var eventDto = new EventDto { Id = "TestEvent", Type = "Invoiced" };
            var eventAddedDto = new EventAddedDto { OrderId = orderId, PreviousStatus = "PaymentReceived", NewStatus = "Invoiced" };
            _mockOrderService.Setup(s => s.AddEventToOrderAsync(orderId, eventDto)).ReturnsAsync(eventAddedDto);

            var result = await _ordersController.AddEventAsync(orderId, eventDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(eventAddedDto, okResult.Value);
        }

        [Fact]
        public async Task AddEventAsync_Returns500_WhenServiceThrows()
        {
            var orderId = 10;
            var eventDto = new EventDto { Id = "TestEvent", Type = "Invoiced" };
            _mockOrderService.Setup(s => s.AddEventToOrderAsync(orderId, eventDto)).ThrowsAsync(new DataAccessException("OrderService", new Exception()));

            var result = await _ordersController.AddEventAsync(orderId, eventDto);
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Ocurrió un error al ingresar un nuevo evento en el sistema.", serverError.Value);
        }

        #endregion
    }
}
