using OrderService.Domain;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Responses;

namespace OrderService.Application.Services.Interfaces
{
    public interface IEventValidationService
    {
        /// <summary>
        /// Creates new order event
        /// </summary>
        /// <returns>Event object.</returns>
        Event CreateNewOrderEvent();

        /// <summary>
        /// Creates event added dto
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="previousStatus">Previous status.</param>
        /// <param name="newStatus">New status.</param>
        /// <returns>Event added dto object.</returns>
        EventAddedDto CreateEventAddedDto(int orderId, string previousStatus, string newStatus);

        /// <summary>
        /// Checks if the event is valid and not processed
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="eventDto">Event dto object.</param>
        /// <returns>A tuple with: Event validation result; and if it's was not processed.</returns>
        Task<(bool, bool)> IsEventValidAndNotProcessedAsync(int orderId, EventDto eventDto);
    }
}
