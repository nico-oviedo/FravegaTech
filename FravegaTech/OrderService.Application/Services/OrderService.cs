using AutoMapper;
using OrderService.Application.Services.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Domain;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;

namespace OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventService _eventService;
        private readonly IOrderValidationService _orderValidationService;
        private readonly IOrderCreationService _orderCreationService;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IEventService eventService,
            IOrderValidationService orderValidationService, IOrderCreationService orderCreationService, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _orderValidationService = orderValidationService ?? throw new ArgumentNullException(nameof(orderValidationService));
            _orderCreationService = orderCreationService ?? throw new ArgumentNullException(nameof(orderCreationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<OrderTranslatedDto> GetAndTranslateOrderAsync(int orderId)
        {
            Order order = await _orderRepository.GetByOrderIdAsync(orderId);
            return _mapper.Map<OrderTranslatedDto>(order);
        }

        /// <inheritdoc/>
        public async Task<List<OrderDto>> SearchOrdersAsync(int orderId, string documentNumber, string status,
            string createdOnFrom, string createdOnTo)
        {
            //Armo filtros con los parametros que me llegan
            Dictionary<string, object> filters = new Dictionary<string, object>();

            var orders = await _orderRepository.SearchOrdersAsync(filters);
            return _mapper.Map<List<OrderDto>>(orders);
        }

        /// <inheritdoc/>
        public async Task<OrderCreatedDto> AddOrderAsync(OrderRequestDto orderRequestDto)
        {
            try
            {
                if (!await _orderValidationService.IsOrderValidAsync(orderRequestDto))
                {
                    //Logueo error
                    //genero objeto respuesta con mensaje
                    return null;
                }

                Order newOrder = await _orderCreationService.CreateNewOrderAsync(orderRequestDto);
                string? idOrderAdded = await _orderRepository.AddOrderAsync(newOrder);

                if (idOrderAdded is null)
                {
                    //Logueo error
                    //genero objeto respuesta con mensaje
                    return null;
                }

                return _mapper.Map<OrderCreatedDto>(newOrder);
            }
            catch (Exception ex)
            {
                return null;
            }
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
