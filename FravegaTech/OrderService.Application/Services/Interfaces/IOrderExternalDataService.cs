using OrderService.Domain;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;

namespace OrderService.Application.Services.Interfaces
{
    public interface IOrderExternalDataService
    {
        /// <summary>
        /// Gets buyer id by document number
        /// </summary>
        /// <param name="documentNumber">Buyer document number.</param>
        /// <returns>Buyer id.</returns>
        Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber);

        /// <summary>
        /// Gets in parallel buyer dto and list of order products dto from an order
        /// </summary>
        /// <param name="order">Order object.</param>
        /// <returns>Tuple with buyer dto and list of order product dto.</returns>
        Task<(BuyerDto?, List<OrderProductDto>)> GetBuyerDtoAndOrderProductsDtoFromOrderAsync(Order order);

        /// <summary>
        /// Gets in parallel order id, buyer id and list of order products from order request dto
        /// </summary>
        /// <param name="orderRequestDto">Order request dto.</param>
        /// <returns>Tuple with order id, buyer id and list of order product.</returns>
        Task<(int, string?, List<OrderProduct>)> GetDataFromOrderRequestDtoAsync(OrderRequestDto orderRequestDto);
    }
}
