using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;

namespace OrderService.Application.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Gets and translate order
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Order translated object.</returns>
        Task<OrderTranslatedDto> GetAndTranslateOrderAsync(int orderId);

        /// <summary>
        /// Searchs orders given some filters
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="documentNumber">Buyer document number.</param>
        /// <param name="status">Order status.</param>
        /// <param name="createdOnFrom">Order created from.</param>
        /// <param name="createdOnTo">Order created to.</param>
        /// <returns>List of orders dto.</returns>
        Task<IEnumerable<OrderDto>> SearchOrdersAsync(int orderId, string documentNumber, string status,
            string createdOnFrom, string createdOnTo);

        /// <summary>
        /// Inserts new order
        /// </summary>
        /// <param name="orderRequestDto">Order request dto object.</param>
        /// <returns>Order created dto object.</returns>
        Task<OrderCreatedDto> InsertNewOrderAsync(OrderRequestDto orderRequestDto);

        /// <summary>
        /// Add event to an order
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="eventDto">Event dto object.</param>
        /// <returns>Event added dto object.</returns>
        Task<EventAddedDto> AddEventAsync(int orderId, EventDto eventDto);
    }
}
