using SharedKernel.Dtos.Requests;

namespace OrderService.Application.Services.Interfaces
{
    public interface IOrderValidationService
    {
        /// <summary>
        /// Checks if the order is valid or not
        /// </summary>
        /// <param name="orderRequestDto">Order request dto object.</param>
        /// <returns>True if it's a valid order.</returns>
        Task<bool> IsOrderValidAsync(OrderRequestDto orderRequestDto);
    }
}
