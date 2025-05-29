using AutoMapper;
using BuyerService.Data.Repositories;
using BuyerService.Domain;
using SharedKernel.Dtos;

namespace BuyerService.Application.Services
{
    public class BuyerService : IBuyerService
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IMapper _mapper;

        public BuyerService(IBuyerRepository buyerRepository, IMapper mapper)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<BuyerDto> GetBuyerByIdAsync(string buyerId)
        {
            Buyer buyer = await _buyerRepository.GetBuyerByIdAsync(buyerId);
            return _mapper.Map<BuyerDto>(buyer);
        }

        /// <inheritdoc/>
        public async Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber)
        {
            return await _buyerRepository.GetBuyerIdByDocumentNumberAsync(documentNumber);
        }

        /// <inheritdoc/>
        public async Task<string?> AddBuyerAsync(BuyerDto buyerDto)
        {
            string? buyerId = await _buyerRepository.GetBuyerIdByDocumentNumberAsync(buyerDto.DocumentNumber);

            if (buyerId is not null)
                return buyerId;

            Buyer buyer = _mapper.Map<Buyer>(buyerDto);
            return await _buyerRepository.AddBuyerAsync(buyer);
        }
    }
}
