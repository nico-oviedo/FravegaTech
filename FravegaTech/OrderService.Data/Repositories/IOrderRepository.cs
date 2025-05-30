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
        /// Validates if it's unique the external reference in channel
        /// </summary>
        /// <param name="externalReferenceId">External reference id.</param>
        /// <param name="channel">Source channel.</param>
        /// <returns>True if external reference does not exist in channel, False the opposite.</returns>
        Task<bool> IsUniqueExternalReferenceInChannelAsync(string externalReferenceId, SourceChannel channel);

        /// <summary>
        /// Validates if it's unique event id in order
        /// </summary>
        /// <param name="orderId"><Order id./param>
        /// <param name="eventId">Event id.</param>
        /// <returns>True if event id does not exist in order, False the opposite.</returns>
        Task<bool> IsUniqueEventIdAsync(int orderId, string eventId);

        /// <summary>
        /// Validates if the event was already processed
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="eventType">Event type.</param>
        /// <returns>True if event was already processed, False the opposite.</returns>
        Task<bool> IsEventAlreadyProcessedAsync(int orderId, OrderStatus eventType);

        /// <summary>
        /// Gets order status
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Order status.</returns>
        Task<OrderStatus> GetOrderStatusAsync(int orderId);

        /// <summary>
        /// Adds new order
        /// </summary>
        /// <param name="order">Order object.</param>
        /// <returns>Added order id.</returns>
        Task<string> AddOrderAsync(Order order);

        /// <summary>
        /// Adds new event to an order
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="newEvent">New event.</param>
        /// <returns>True if the event was added successfully, False if it's not.</returns>
        Task<bool> AddEventAsync(int orderId, Event newEvent);

        /// <summary>
        /// Updates order status
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="newStatus">New status.</param>
        /// <returns>True if the order was updated successfully, False if it's not.</returns>
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);

        /// <summary>
        /// Searchs orders given some filters
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="buyerId">Order buyer id.</param>
        /// <param name="status">Order status.</param>
        /// <param name="createdOnFrom">Order created from.</param>
        /// <param name="createdOnTo">Order created to.</param>
        /// <returns>List of orders.</returns>
        Task<List<Order>> SearchOrdersAsync(int? orderId, string? buyerId, OrderStatus? status,
            DateTime? createdOnFrom, DateTime? createdOnTo);
    }
}