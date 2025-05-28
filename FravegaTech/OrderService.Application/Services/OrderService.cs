using AutoMapper;
using OrderService.Application.Services.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;

namespace OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventValidationService _eventValidationService;
        private readonly IOrderValidationService _orderValidationService;
        private readonly IOrderCreationService _orderCreationService;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IEventValidationService eventValidationService,
            IOrderValidationService orderValidationService, IOrderCreationService orderCreationService, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _eventValidationService = eventValidationService ?? throw new ArgumentNullException(nameof(eventValidationService));
            _orderValidationService = orderValidationService ?? throw new ArgumentNullException(nameof(orderValidationService));
            _orderCreationService = orderCreationService ?? throw new ArgumentNullException(nameof(orderCreationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<OrderTranslatedDto> GetFullOrderAsync(int orderId)
        {
            Order order = await _orderRepository.GetByOrderIdAsync(orderId);
            var (buyerDto, orderProductsDtoList) = await _orderCreationService.GetBuyerAndProductsForOrderAsync(order);

            var fullOrder = _mapper.Map<OrderTranslatedDto>(order);
            fullOrder.Buyer = buyerDto;
            fullOrder.Products = orderProductsDtoList;

            return fullOrder;
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
        public async Task<EventAddedDto> AddEventToOrderAsync(int orderId, EventDto eventDto)
        {
            try
            {
                var (isEventValid, isEventNotProcessed) = await _eventValidationService.IsEventValidAndNotProcessedAsync(orderId, eventDto);

                if (isEventValid && isEventNotProcessed)
                    return await ProcessNewEventAsync(orderId, eventDto);

                if (!isEventValid)
                {
                    //Logueo error
                    //genero objeto respuesta con mensaje
                    return null;
                }
                else
                    return _eventValidationService.CreateEventAddedDto(orderId, eventDto.Type, eventDto.Type);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Process new event
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="eventDto">Event dto object.</param>
        /// <returns>Event added dto object.</returns>
        private async Task<EventAddedDto> ProcessNewEventAsync(int orderId, EventDto eventDto)
        {
            Event newEvent = _mapper.Map<Event>(eventDto);
            bool wasEventAdded = await _orderRepository.AddEventAsync(orderId, newEvent);

            if (!wasEventAdded)
            {
                //Logueo error
                //genero objeto respuesta con mensaje
                return null;
            }

            OrderStatus? previousStatus = await _orderRepository.GetOrderStatusAsync(orderId);
            await _orderRepository.UpdateOrderStatusAsync(orderId, newEvent.Type);

            return _eventValidationService.CreateEventAddedDto(orderId, previousStatus.Value.ToString(), newEvent.Type.ToString());
        }
    }
}
