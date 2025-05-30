using AutoMapper;
using BuyerService.Data.Repositories;
using BuyerService.Domain;
using Microsoft.Extensions.Logging;
using SharedKernel.Dtos;
using SharedKernel.Exceptions;

namespace BuyerService.Application.Services
{
    public class BuyerService : IBuyerService
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BuyerService> _logger;

        public BuyerService(IBuyerRepository buyerRepository, IMapper mapper, ILogger<BuyerService> logger)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<BuyerDto> GetBuyerByIdAsync(string buyerId)
        {
            try
            {
                _logger.LogInformation($"Trying to get Buyer with id: {buyerId}.");
                Buyer buyer = await _buyerRepository.GetBuyerByIdAsync(buyerId);

                if (buyer is null)
                {
                    _logger.LogError($"Buyer with id {buyerId} was not found.");
                    throw new NotFoundException(nameof(buyer), $"{GetType().Name}:{nameof(GetBuyerByIdAsync)}");
                }

                BuyerDto buyerDto = _mapper.Map<BuyerDto>(buyer);
                _logger.LogInformation($"Successfully get Buyer with id: {buyerId}.");
                return buyerDto;
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, $"Failed to map Buyer to BuyerDto. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber)
        {
            _logger.LogInformation($"Trying to get BuyerId with document number: {documentNumber}.");
            return await _buyerRepository.GetBuyerIdByDocumentNumberAsync(documentNumber);
        }

        /// <inheritdoc/>
        public async Task<string> AddBuyerAsync(BuyerDto buyerDto)
        {
            try
            {
                _logger.LogInformation("Trying to add Buyer.");
                string? buyerId = await _buyerRepository.GetBuyerIdByDocumentNumberAsync(buyerDto.DocumentNumber);

                if (buyerId is not null)
                {
                    _logger.LogInformation($"Buyer with document number: {buyerDto.DocumentNumber} already exists. Returning BuyerId: {buyerId}.");
                    return buyerId;
                }

                Buyer buyer = _mapper.Map<Buyer>(buyerDto);
                buyerId = await _buyerRepository.AddBuyerAsync(buyer);

                _logger.LogInformation($"Buyer added successfully. Returning BuyerId: {buyerId}.");
                return buyerId;
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, $"Failed to map BuyerDto to Buyer. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
    }
}
