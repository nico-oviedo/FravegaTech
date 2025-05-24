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
        public async Task<string?> GetBuyerIdOrInsertNewBuyerAsync(BuyerDto buyerDto)
        {
            string? buyerId = await _buyerRepository.GetBuyerIdByDocumentNumberAsync(buyerDto.DocumentNumber);

            if (buyerId is null)
            {
                Buyer buyer = _mapper.Map<Buyer>(buyerDto);
                buyerId = await _buyerRepository.InsertBuyerAsync(buyer);
            }

            return buyerId;
        }
    }
}
