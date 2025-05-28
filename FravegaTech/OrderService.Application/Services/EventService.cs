using OrderService.Application.Services.Interfaces;
using OrderService.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Application.Services
{
    public class EventService : IEventService
    {
        /// <inheritdoc/>
        public Event CreateNewOrderEvent() 
        {
            return new Event()
            {
                EventId = "event-001", //Creo q voy a tener que crear otro counterDeEventos
                Type = OrderStatus.Created,
                Date = DateTime.UtcNow,
                User = "System"
            };
        }

    }
}
