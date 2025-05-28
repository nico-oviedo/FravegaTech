using OrderService.Domain;

namespace OrderService.Application.Services.Interfaces
{
    public interface IEventService
    {
        /// <summary>
        /// Creates new order event
        /// </summary>
        /// <returns>Event object.</returns>
        Event CreateNewOrderEvent();
    }
}
