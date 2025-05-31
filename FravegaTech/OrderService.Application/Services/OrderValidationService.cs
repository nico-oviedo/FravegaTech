using Microsoft.Extensions.Logging;
using OrderService.Application.Services.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Domain.Enums;
using SharedKernel.Dtos.Requests;

namespace OrderService.Application.Services
{
    public class OrderValidationService : IOrderValidationService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderValidationService> _logger;

        public OrderValidationService(IOrderRepository orderRepository, ILogger<OrderValidationService> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<bool> IsOrderValidAsync(OrderRequestDto orderRequestDto)
        {
            try
            {
                _logger.LogInformation("Starting Order validation.");

                Enum.TryParse<SourceChannel>(orderRequestDto.Channel, true, out var orderChannel);
                Task<bool> isExternalRefUniqueTask = _orderRepository.IsUniqueExternalReferenceInChannelAsync(orderRequestDto.ExternalReferenceId, orderChannel);
                Task<bool> isProperTotalTask = Task.Run(() => IsProperTotalValue(orderRequestDto));

                await Task.WhenAll(isExternalRefUniqueTask, isProperTotalTask);

                bool isExternalRefUnique = await isExternalRefUniqueTask;
                bool isProperTotal = await isProperTotalTask;

                _logger.LogInformation("Finish Order validation.");
                return isExternalRefUnique && isProperTotal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to validate Order. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Validates if order total value is correct
        /// </summary>
        /// <param name="orderRequestDto">Order request dto object.</param>
        /// <returns>True if it's proper order total value, False the opposite.</returns>
        private bool IsProperTotalValue(OrderRequestDto orderRequestDto)
        {
            _logger.LogInformation("Validating Order total value.");

            decimal totalValueProducts = 0;
            foreach (var product in orderRequestDto.Products)
            {
                totalValueProducts += product.Price * product.Quantity;
            }

            return totalValueProducts == orderRequestDto.TotalValue;
        }
    }
}
