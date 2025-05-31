using AutoMapper;
using BuyerService.Data.Repositories;
using BuyerService.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel.Dtos;
using SharedKernel.Exceptions;
using BuyerApplication = BuyerService.Application.Services;

namespace BuyerService.Application.Tests.Services
{
    public class BuyerServiceTests
    {
        private readonly Mock<IBuyerRepository> _mockBuyerRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<BuyerApplication.BuyerService>> _mockLogger;
        private readonly BuyerApplication.BuyerService _buyerService;

        public BuyerServiceTests()
        {
            _mockBuyerRepository = new Mock<IBuyerRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<BuyerApplication.BuyerService>>();
            _buyerService = new BuyerApplication.BuyerService(_mockBuyerRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetBuyerByIdAsync_ReturnsBuyerDto_WhenBuyerExists()
        {
            var buyerId = "ABC123";
            var buyer = new Buyer { _id = buyerId, FirstName = "Silvia", LastName = "Perez", DocumentNumber = "41.223.457" };
            var buyerDto = new BuyerDto { FirstName = "Silvia", LastName = "Perez", DocumentNumber = "41.223.457" };

            _mockBuyerRepository.Setup(r => r.GetBuyerByIdAsync(buyerId)).ReturnsAsync(buyer);
            _mockMapper.Setup(m => m.Map<BuyerDto>(buyer)).Returns(buyerDto);

            var result = await _buyerService.GetBuyerByIdAsync(buyerId);

            Assert.NotNull(result);
            Assert.Equal(buyer.DocumentNumber, result.DocumentNumber);
        }

        [Fact]
        public async Task GetBuyerByIdAsync_ThrowsNotFoundException_WhenBuyerDoesNotExist()
        {
            var buyerId = "HGF334";
            _mockBuyerRepository.Setup(r => r.GetBuyerByIdAsync(buyerId)).ReturnsAsync((Buyer)null!);

            await Assert.ThrowsAsync<NotFoundException>(() => _buyerService.GetBuyerByIdAsync(buyerId));
        }

        [Fact]
        public async Task GetBuyerIdByDocumentNumberAsync_ReturnsId_WhenFound()
        {
            var document = "54.332.556";
            _mockBuyerRepository.Setup(r => r.GetBuyerIdByDocumentNumberAsync(document)).ReturnsAsync("ABC123");

            var result = await _buyerService.GetBuyerIdByDocumentNumberAsync(document);

            Assert.Equal("ABC123", result);
        }

        [Fact]
        public async Task AddBuyerAsync_ReturnsExistingBuyerId_WhenAlreadyExists()
        {
            var dto = new BuyerDto { FirstName = "Silvia", LastName = "Perez", DocumentNumber = "41.223.457" };
            _mockBuyerRepository.Setup(r => r.GetBuyerIdByDocumentNumberAsync(dto.DocumentNumber)).ReturnsAsync("KJG456");

            var result = await _buyerService.AddBuyerAsync(dto);

            Assert.Equal("KJG456", result);
            _mockBuyerRepository.Verify(r => r.AddBuyerAsync(It.IsAny<Buyer>()), Times.Never);
        }

        [Fact]
        public async Task AddBuyerAsync_AddsBuyerAndReturnsNewId_WhenNotExists()
        {
            var buyerDto = new BuyerDto { FirstName = "Silvia", LastName = "Perez", DocumentNumber = "41.223.457" };
            var buyer = new Buyer { _id = "AAA027", FirstName = "Silvia", LastName = "Perez", DocumentNumber = "41.223.457" };

            _mockBuyerRepository.Setup(r => r.GetBuyerIdByDocumentNumberAsync(buyerDto.DocumentNumber)).ReturnsAsync((string)null!);
            _mockMapper.Setup(m => m.Map<Buyer>(buyerDto)).Returns(buyer);
            _mockBuyerRepository.Setup(r => r.AddBuyerAsync(buyer)).ReturnsAsync("AAA027");

            var result = await _buyerService.AddBuyerAsync(buyerDto);

            Assert.Equal("AAA027", result);
        }
    }
}
