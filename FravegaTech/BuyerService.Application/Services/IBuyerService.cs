using BuyerService.Domain;
using SharedKernel.Dtos;

namespace BuyerService.Application.Services
{
    public interface IBuyerService
    {
        /// <summary>
        /// Gets buyer dto by id
        /// </summary>
        /// <param name="buyerId">Buyer id.</param>
        /// <returns>Buyer dto object.</returns>
        Task<BuyerDto> GetBuyerByIdAsync(string buyerId);

        /// <summary>
        /// Gets buyer id or insert new buyer
        /// </summary>
        /// <param name="buyerDto">Buyer dto object.</param>
        /// <returns>Buyer id.</returns>
        Task<string?> GetBuyerIdOrInsertNewBuyerAsync(BuyerDto buyerDto);
    }
}
