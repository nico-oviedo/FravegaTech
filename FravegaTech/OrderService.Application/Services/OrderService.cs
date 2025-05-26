using AutoMapper;
using CounterService.Services;
using OrderService.Data.Repositories;
using OrderService.Domain;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;
using SharedKernel.ServiceClients;

namespace OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICounterService _counterService;
        private readonly IEventService _eventService;
        private readonly BuyerServiceClient _buyerServiceClient;
        private readonly ProductServiceClient _productServiceClient;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, ICounterService counterService, IEventService eventService,
            BuyerServiceClient buyerServiceClient, ProductServiceClient productServiceClient, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _counterService = counterService ?? throw new ArgumentNullException(nameof(counterService));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _buyerServiceClient = buyerServiceClient ?? throw new ArgumentNullException(nameof(buyerServiceClient));
            _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<OrderTranslatedDto> GetAndTranslateOrderAsync(int orderId)
        {
            Order order = await _orderRepository.GetOrderAsync(orderId);
            return _mapper.Map<OrderTranslatedDto>(order);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrderDto>> SearchOrdersAsync(int orderId, string documentNumber, string status,
            string createdOnFrom, string createdOnTo)
        {
            //Armo filtros con los parametros que me llegan
            Dictionary<string, object> filters = new Dictionary<string, object>();

            var orders = await _orderRepository.SearchOrdersAsync(filters);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        /// <inheritdoc/>
        public async Task<OrderCreatedDto> InsertNewOrderAsync(OrderRequestDto orderRequestDto)
        {
            //Validar que la clave: ExternalReferenceID + Channel no exista en la base de datos

            //Validar que totalValue coincida con la sumatoria de los productos

            //Inserto comprador si no existe, si existe obtengo el BuyerId

            //Inserto producto si no existe, si existe obtengo el ProductId

            //Obtener el OrderId secuencial

            Order order = _mapper.Map<Order>(orderRequestDto);
            /*
                BuyerId = "", //obtener
                Products = null, //mapearlos
                Events = null //servicio de eventos que te cree uno
             */

            //return await _orderRepository.InsertOrderAsync(order);

            return null;
        }

        /// <inheritdoc/>
        public async Task<EventAddedDto> AddEventAsync(int orderId, EventDto eventDto)
        {
            //Validar que el id sea unico

            //Validar que la transicion de estado sea valida, si ya fue procesado ignorarlo y no hacer nada

            //Guardar evento en la orden

            return null;
        }
    }
}
