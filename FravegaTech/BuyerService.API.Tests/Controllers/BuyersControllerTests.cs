using BuyerService.API.Controllers;
using BuyerService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel.Dtos;
using SharedKernel.Exceptions;

namespace BuyerService.API.Tests.Controllers
{
    public class BuyersControllerTests
    {
        private readonly Mock<IBuyerService> _mockBuyerService;
        private readonly Mock<ILogger<BuyersController>> _mockLogger;
        private readonly BuyersController _buyersController;

        public BuyersControllerTests()
        {
            _mockBuyerService = new Mock<IBuyerService>();
            _mockLogger = new Mock<ILogger<BuyersController>>();
            _buyersController = new BuyersController(_mockBuyerService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BuyersController(null!, _mockLogger.Object));
            Assert.Throws<ArgumentNullException>(() => new BuyersController(_mockBuyerService.Object, null!));
        }

        #region GetAsync

        [Fact]
        public async Task GetAsync_ReturnsOk_WhenBuyerExists()
        {
            var buyerId = "123";
            var buyerDto = new BuyerDto { FirstName = "Pedro", LastName = "Rodriguez", DocumentNumber = "28.334.221" };
            _mockBuyerService.Setup(s => s.GetBuyerByIdAsync(buyerId))
                .ReturnsAsync(buyerDto);

            var result = await _buyersController.GetAsync(buyerId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(buyerDto, okResult.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsBadRequest_WhenIdIsNullOrEmpty()
        {
            var result = await _buyersController.GetAsync("");
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id del comprador es requerido.", badRequest.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound_WhenBuyerNotFound()
        {
            _mockBuyerService.Setup(s => s.GetBuyerByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new NotFoundException("Buyer", "BuyerService"));

            var result = await _buyersController.GetAsync("123");
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Comprador no fue encontrado.", notFound.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenDataAccessException()
        {
            _mockBuyerService.Setup(s => s.GetBuyerByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new DataAccessException("BuyerService", new Exception()));

            var result = await _buyersController.GetAsync("123");
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Ocurrió un error al obtener los datos del comprador.", serverError.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenUnhandledException()
        {
            _mockBuyerService.Setup(s => s.GetBuyerByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var result = await _buyersController.GetAsync("123");
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Un error interno ha ocurrido.", serverError.Value);
        }

        #endregion

        #region GetByDocumentNumberAsync

        [Fact]
        public async Task GetByDocumentNumberAsync_ReturnsOk_WhenBuyerIdFound()
        {
            var docNumber = "28.334.221";
            _mockBuyerService.Setup(s => s.GetBuyerIdByDocumentNumberAsync(docNumber))
                .ReturnsAsync("AAB789");

            var result = await _buyersController.GetByDocumentNumberAsync(docNumber);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("AAB789", okResult.Value);
        }

        [Fact]
        public async Task GetByDocumentNumberAsync_ReturnsBadRequest_WhenDocumentNumberIsEmpty()
        {
            var result = await _buyersController.GetByDocumentNumberAsync("");
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Número de documento del comprador es requerido.", badRequest.Value);
        }

        [Fact]
        public async Task GetByDocumentNumberAsync_Returns500_WhenDataAccessException()
        {
            _mockBuyerService.Setup(s => s.GetBuyerIdByDocumentNumberAsync(It.IsAny<string>()))
                .ThrowsAsync(new DataAccessException("BuyerRepository", new Exception()));

            var result = await _buyersController.GetByDocumentNumberAsync("28.334.221");
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Ocurrió un error al obtener el id del comprador.", serverError.Value);
        }

        [Fact]
        public async Task GetByDocumentNumberAsync_Returns500_WhenUnhandledException()
        {
            _mockBuyerService.Setup(s => s.GetBuyerIdByDocumentNumberAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var result = await _buyersController.GetByDocumentNumberAsync("28.334.221");
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Un error interno ha ocurrido.", serverError.Value);
        }

        #endregion

        #region PostAsync

        [Fact]
        public async Task PostAsync_ReturnsOk_WhenBuyerAdded()
        {
            var buyerDto = new BuyerDto { FirstName = "Pedro", LastName = "Rodriguez", DocumentNumber = "28.334.221" };
            _mockBuyerService.Setup(s => s.AddBuyerAsync(buyerDto)).ReturnsAsync("ABC456");

            var result = await _buyersController.PostAsync(buyerDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("ABC456", okResult.Value);
        }

        [Fact]
        public async Task PostAsync_ReturnsInternalServerError_WhenDataAccessException()
        {
            var buyerDto = new BuyerDto { FirstName = "Pedro", LastName = "Rodriguez", DocumentNumber = "28.334.221" };
            _mockBuyerService.Setup(s => s.AddBuyerAsync(buyerDto)).ThrowsAsync(new DataAccessException("BuyerService", new Exception()));

            var result = await _buyersController.PostAsync(buyerDto);
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Ocurrió un error al ingresar un nuevo comprador en el sistema.", serverError.Value);
        }

        [Fact]
        public async Task PostAsync_ReturnsInternalServerError_WhenUnhandledException()
        {
            var buyerDto = new BuyerDto { FirstName = "Pedro", LastName = "Rodriguez", DocumentNumber = "28.334.221" };
            _mockBuyerService.Setup(s => s.AddBuyerAsync(buyerDto)).ThrowsAsync(new Exception());

            var result = await _buyersController.PostAsync(buyerDto);
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Un error interno ha ocurrido.", serverError.Value);
        }

        #endregion
    }
}
