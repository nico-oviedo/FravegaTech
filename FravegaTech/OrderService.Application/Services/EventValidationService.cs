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

        public EventValidationService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
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
        public async Task<(bool, bool)> IsEventValidAndNotProcessedAsync(int orderId, EventDto eventDto)
        {
            try
            {
                bool eventTypeParsed = Enum.TryParse<OrderStatus>(eventDto.Type, out var eventType);
                if (!eventTypeParsed)
                {
                    //loguear error
                    return (false, false);
                }

                Task<bool> isUniqueEventIdTask = _orderRepository.IsUniqueEventIdAsync(orderId, eventDto.Id);
                Task<bool> isValidTransitionTask = GetOrderStatusAndValidateTransitionAsync(orderId, eventType);
                Task<bool> isEventAlreadyProcessedTask = _orderRepository.IsEventAlreadyProcessedAsync(orderId, eventType);

                await Task.WhenAll(isUniqueEventIdTask, isValidTransitionTask, isEventAlreadyProcessedTask);

                bool isUniqueEventId = await isUniqueEventIdTask;
                bool isValidTransition = await isValidTransitionTask;
                bool isEventAlreadyProcessed = await isEventAlreadyProcessedTask;

                return (isUniqueEventId && isValidTransition, !isEventAlreadyProcessed);
            }
            catch (Exception ex)
            {
                return (false, false);
            }
        }

        /// <summary>
        /// Gets order status and validates transition
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="newStatus">New status.</param>
        /// <returns>True if it's a valid status transition, False if it's not.</returns>
        private async Task<bool> GetOrderStatusAndValidateTransitionAsync(int orderId, OrderStatus newStatus)
        {
            OrderStatus? currentStatus = await _orderRepository.GetOrderStatusAsync(orderId);
            return IsValidTransition(currentStatus.Value, newStatus);
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
