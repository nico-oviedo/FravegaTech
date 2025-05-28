using OrderService.Domain;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;

namespace OrderService.Application.Services.Interfaces
{
    public interface IOrderCreationService
    {
        /// <summary>
        /// Creates new order object
        /// </summary>
        /// <param name="orderRequestDto">Order request dto.</param>
        /// <returns>Order object.</returns>
        Task<Order> CreateNewOrderAsync(OrderRequestDto orderRequestDto);

        /// <summary>
        /// Gets buyer dto and list of order products dto from an order
        /// </summary>
        /// <param name="order">Order object.</param>
        /// <returns>Tuple with buyer dto and list of order product dto.</returns>
        Task<(BuyerDto?, List<OrderProductDto>)> GetBuyerAndProductsForOrderAsync(Order order);
    }
}
