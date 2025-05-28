using OrderService.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Data.Repositories
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Gets order by order id
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Order object.</returns>
        Task<Order> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// Searchs orders given some filters
        /// </summary>
        /// <param name="filters">Query filters.</param>
        /// <returns>List of orders.</returns>
        Task<List<Order>> SearchOrdersAsync(Dictionary<string, object> filters);

        /// <summary>
        /// Validates if it's unique the external reference in channel
        /// </summary>
        /// <param name="externalReferenceId">External reference id.</param>
        /// <param name="channel">Source channel.</param>
        /// <returns>True if external reference does not exist in channel, False the opposite.</returns>
        Task<bool> IsUniqueExternalReferenceInChannelAsync(string externalReferenceId, SourceChannel channel);

        /// <summary>
        /// Adds new order
        /// </summary>
        /// <param name="order">Order object.</param>
        /// <returns>Added order id.</returns>
        Task<string?> AddOrderAsync(Order order);
    }
}
