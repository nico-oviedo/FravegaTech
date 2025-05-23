using BuyerService.Data.Repositories;
using BuyerService.Domain;
using Shared.Dtos;

namespace BuyerService.Application.Services
{
    public class BuyerService : IBuyerService
    {
        private readonly IBuyerRepository _buyerRepository;

        public BuyerService(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        }

        /// <inheritdoc/>
        public async Task<string?> GetOrInsertNewBuyer(BuyerDto buyerDto)
        {
            string? buyerId = await _buyerRepository.GetBuyerIdByDocumentNumber(buyerDto.DocumentNumber);

            if (buyerId is null)
            {
                //TODO: **Quizas agregue un mapeo automatico luego**
                Buyer buyer = new Buyer()
                {
                    FirstName = buyerDto.FirstName,
                    LastName = buyerDto.LastName,
                    DocumentNumber = buyerDto.DocumentNumber,
                    Phone = buyerDto.Phone
                };

                buyerId = await _buyerRepository.InsertBuyer(buyer);
            }

            return buyerId;
        }
    }
}
