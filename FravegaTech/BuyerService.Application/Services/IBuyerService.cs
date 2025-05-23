using Shared.Dtos;

namespace BuyerService.Application.Services
{
    public interface IBuyerService
    {
        /// <summary>
        /// Gets or insert new buyer
        /// </summary>
        /// <param name="buyerDto">Buyer dto object.</param>
        /// <returns>Buyer id.</returns>
        Task<string?> GetOrInsertNewBuyer(BuyerDto buyerDto);
    }
}
