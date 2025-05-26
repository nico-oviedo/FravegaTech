using BuyerService.Domain;

namespace BuyerService.Data.Repositories
{
    public interface IBuyerRepository
    {
        /// <summary>
        /// Gets buyer by id
        /// </summary>
        /// <param name="buyerId">Buyer id.</param>
        /// <returns>Buyer object.</returns>
        Task<Buyer> GetBuyerByIdAsync(string buyerId);

        /// <summary>
        /// Gets buyer id by document number
        /// </summary>
        /// <param name="documentNumber">Buyer document number.</param>
        /// <returns>Buyer id.</returns>
        Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber);

        /// <summary>
        /// Adds new buyer
        /// </summary>
        /// <param name="buyer">Buyer object.</param>
        /// <returns>Added buyer id.</returns>
        Task<string?> AddBuyerAsync(Buyer buyer);
    }
}
