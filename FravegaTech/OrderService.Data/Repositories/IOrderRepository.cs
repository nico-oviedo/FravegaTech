using OrderService.Domain;

namespace OrderService.Data.Repositories
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Gets order by order id
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Order object.</returns>
        Task<Order> GetOrderAsync(int orderId);

        /// <summary>
        /// Searchs orders given some filters
        /// </summary>
        /// <param name="filters">Query filters.</param>
        /// <returns>List of orders.</returns>
        Task<IEnumerable<Order>> SearchOrdersAsync(Dictionary<string, object> filters);

        /// <summary>
        /// Inserts new order
        /// </summary>
        /// <param name="order">Order object.</param>
        /// <returns>Inserted order id.</returns>
        Task<string?> InsertOrderAsync(Order order);
    }
}
