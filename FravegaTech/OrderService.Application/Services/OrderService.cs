using CounterService.Services;
using OrderService.Data.Repositories;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Responses;

namespace OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICounterService _counterService;
        private readonly IEventService _eventService;

        public OrderService(IOrderRepository orderRepository, ICounterService counterService, IEventService eventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _counterService = counterService ?? throw new ArgumentNullException(nameof(counterService));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        /// <inheritdoc/>
        public async Task<OrderTranslatedDto> GetAndTranslateOrderAsync(int orderId)
        {
            Order order = await _orderRepository.GetOrderAsync(orderId);

            //Mapear de Orden a OrderTranslatedDto y traducirla

            return null;
        }

        /// <inheritdoc/>
        public async Task<OrderDto> SearchOrdersAsync(int orderId, string documentNumber, string status,
            string createdOnFrom, string createdOnTo)
        {
            //Armo filtros con los parametros que me llegan
            Dictionary<string, object> filters = new Dictionary<string, object>();

            var orders = _orderRepository.SearchOrdersAsync(filters);

            //Mapeo a ordenDto

            return null;
        }

        /// <inheritdoc/>
        public async Task<string?> InsertNewOrderAsync(OrderDto orderDto)
        {
            //Validar que la clave: ExternalReferenceID + Channel no exista en la base de datos

            //Validar que totalValue coincida con la sumatoria de los productos

            //Inserto comprador si no existe, si existe obtengo el BuyerId

            //Inserto producto si no existe, si existe obtengo el ProductId

            //Obtener el OrderId secuencial

            //TODO: **Quizas agregue un mapeo automatico luego**
            Order order = new Order()
            {
                ExternalReferenceId = orderDto.ExternalReferenceId,
                Channel = SourceChannel.Affiliate, //obtener
                PurchaseDate = orderDto.PurchaseDate,
                TotalValue = orderDto.TotalValue,
                BuyerId = "", //obtener
                OrderProducts = null, //mapearlos
                Status = OrderStatus.Created,
                Events = null //servicio de eventos que te cree uno
            };

            return await _orderRepository.InsertOrderAsync(order);
        }

        /// <inheritdoc/>
        public async Task<bool> AddEventAsync(int orderId, EventDto eventDto)
        {
            //Validar que el id sea unico

            //Validar que la transicion de estado sea valida, si ya fue procesado ignorarlo y no hacer nada

            //Guardar evento en la orden

            return false;
        }
    }
}
