using OrderService.Application.Services.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Domain.Enums;
using SharedKernel.Dtos.Requests;

namespace OrderService.Application.Services
{
    public class OrderValidationService : IOrderValidationService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderValidationService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        /// <inheritdoc/>
        public async Task<bool> IsOrderValidAsync(OrderRequestDto orderRequestDto)
        {
            Enum.TryParse<SourceChannel>(orderRequestDto.Channel, out var orderChannel);
            Task<bool> isExternalRefUniqueTask = _orderRepository.IsUniqueExternalReferenceInChannelAsync(orderRequestDto.ExternalReferenceId, orderChannel);
            Task<bool> isProperTotalTask = Task.Run(() => IsProperTotalValue(orderRequestDto));

            await Task.WhenAll(isExternalRefUniqueTask, isProperTotalTask);

            bool isExternalRefUnique = await isExternalRefUniqueTask;
            bool isProperTotal = await isProperTotalTask;

            return isExternalRefUnique && isProperTotal;
        }

        /// <summary>
        /// Validates if order total value is correct
        /// </summary>
        /// <param name="orderRequestDto">Order request dto object.</param>
        /// <returns>True if it's proper order total value, False the opposite.</returns>
        private bool IsProperTotalValue(OrderRequestDto orderRequestDto)
        {
            decimal totalValueProducts = 0;
            foreach (var product in orderRequestDto.Products)
            {
                totalValueProducts += product.Price * product.Quantity;
            }

            return totalValueProducts == orderRequestDto.TotalValue;
        }
    }
}
