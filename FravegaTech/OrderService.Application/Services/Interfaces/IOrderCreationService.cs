using OrderService.Domain;
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
    }
}
