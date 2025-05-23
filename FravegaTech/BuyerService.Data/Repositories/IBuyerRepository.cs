using BuyerService.Domain;

namespace BuyerService.Data.Repositories
{
    public interface IBuyerRepository
    {
        /// <summary>
        /// Gets buyer id by document number
        /// </summary>
        /// <param name="documentNumber">Document number.</param>
        /// <returns>Buyer id.</returns>
        Task<string?> GetBuyerIdByDocumentNumber(string documentNumber);

        /// <summary>
        /// Inserts new buyer
        /// </summary>
        /// <param name="buyer">Buyer object.</param>
        /// <returns>Inserted buyer id.</returns>
        Task<string?> InsertBuyer(Buyer buyer);
    }
}
