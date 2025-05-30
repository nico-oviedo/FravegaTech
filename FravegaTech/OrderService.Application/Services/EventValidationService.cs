using Microsoft.Extensions.Logging;
using OrderService.Application.Services.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Responses;

namespace OrderService.Application.Services
{
    public class EventValidationService : IEventValidationService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly Dictionary<OrderStatus, OrderStatus[]> _validStatusTransitions;
        private readonly ILogger<EventValidationService> _logger;

        public EventValidationService(IOrderRepository orderRepository, ILogger<EventValidationService> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validStatusTransitions = SetValidStatusTransitions();
        }

        /// <inheritdoc/>
        public Event CreateNewOrderEvent()
        {
            return new Event()
            {
                EventId = "event-001",
                Type = OrderStatus.Created,
                Date = DateTime.UtcNow,
                User = "System"
            };
        }

        /// <inheritdoc/>
        public EventAddedDto CreateEventAddedDto(int orderId, string previousStatus, string newStatus)
        {
            return new EventAddedDto()
            {
                OrderId = orderId,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                UpdatedOn = DateTime.UtcNow
            };
        }

        /// <inheritdoc/>
        public (bool, bool) IsEventValidAndNotProcessedAsync(Order order, EventDto eventDto)
        {
            try
            {
                _logger.LogInformation("Starting Event validation.");
                var eventType = Enum.Parse<OrderStatus>(eventDto.Type);

                bool isUniqueEventId = !order.Events.Any(e => e.EventId.ToLower() == eventDto.Id.ToLower());
                bool isValidTransition = IsValidTransition(order.Status, eventType);
                bool isEventAlreadyProcessed = order.Events.Any(e => e.Type == eventType);

                _logger.LogInformation("Finish Event validation.");
                return (isUniqueEventId && isValidTransition, !isEventAlreadyProcessed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to validate Event. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Check if it's a valid status transition
        /// </summary>
        /// <param name="currentStatus">Current status.</param>
        /// <param name="newStatus">New status.</param>
        /// <returns>True if it's a valid status transition, False if it's not</returns>
        private bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return _validStatusTransitions.TryGetValue(currentStatus, out var valids)
                && valids.Contains(newStatus);
        }

        /// <summary>
        /// Sets valid status transitions
        /// </summary>
        /// <returns>Dictionary with valid status transitions.</returns>
        private Dictionary<OrderStatus, OrderStatus[]> SetValidStatusTransitions()
        {
            return new()
            {
                { OrderStatus.Created, new[] { OrderStatus.Created, OrderStatus.PaymentReceived, OrderStatus.Cancelled } },
                { OrderStatus.PaymentReceived, new[] { OrderStatus.PaymentReceived, OrderStatus.Invoiced } },
                { OrderStatus.Invoiced, new[] { OrderStatus.Invoiced, OrderStatus.Returned } },
                { OrderStatus.Returned, new[] { OrderStatus.Returned } },
                { OrderStatus.Cancelled, new[] { OrderStatus.Cancelled } }
            };
        }
    }
}
