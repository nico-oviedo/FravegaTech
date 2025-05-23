using BuyerService.Domain;
using SharedKernel.Dtos;

namespace BuyerService.Application.Services
{
    public interface IBuyerService
    {
        /// <summary>
        /// Gets buyer by id
        /// </summary>
        /// <param name="buyerId">Buyer id.</param>
        /// <returns>Buyer object.</returns>
        Task<Buyer?> GetBuyerByIdAsync(string buyerId);

        /// <summary>
        /// Gets or insert new buyer
        /// </summary>
        /// <param name="buyerDto">Buyer dto object.</param>
        /// <returns>Buyer id.</returns>
        Task<string?> GetOrInsertNewBuyerAsync(BuyerDto buyerDto);
    }
}
