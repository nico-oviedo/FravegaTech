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
        /// Gets buyer id by document number
        /// </summary>
        /// <param name="documentNumber">Buyer document number.</param>
        /// <returns>Buyer id.</returns>
        Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber);

        /// <summary>
        /// Adds new buyer
        /// </summary>
        /// <param name="buyerDto">Buyer dto object.</param>
        /// <returns>Added buyer id.</returns>
        Task<string> AddBuyerAsync(BuyerDto buyerDto);
    }
}
