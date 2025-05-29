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
        private readonly IOrderExternalDataService _orderExternalDataService;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IEventValidationService eventValidationService,
            IOrderValidationService orderValidationService, IOrderExternalDataService orderExternalDataService, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _eventValidationService = eventValidationService ?? throw new ArgumentNullException(nameof(eventValidationService));
            _orderValidationService = orderValidationService ?? throw new ArgumentNullException(nameof(orderValidationService));
            _orderExternalDataService = orderExternalDataService ?? throw new ArgumentNullException(nameof(orderExternalDataService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<OrderTranslatedDto> GetFullOrderAsync(int orderId)
        {
            Order order = await _orderRepository.GetByOrderIdAsync(orderId);
            var (buyerDto, orderProductsDtoList) = await _orderExternalDataService.GetBuyerDtoAndOrderProductsDtoFromOrderAsync(order);

            var fullOrder = _mapper.Map<OrderTranslatedDto>(order);
            fullOrder.Buyer = buyerDto;
            fullOrder.Products = orderProductsDtoList;

            return fullOrder;
        }

        /// <inheritdoc/>
        public async Task<List<OrderDto>> SearchOrdersAsync(int? orderId, string? documentNumber, string? status,
            DateTime? createdOnFrom, DateTime? createdOnTo)
        {
            string? buyerId = documentNumber is not null
                ? await _orderExternalDataService.GetBuyerIdByDocumentNumberAsync(documentNumber)
                : null;

            OrderStatus? orderStatus = status is not null
                && Enum.TryParse<OrderStatus>(status, out var statusOut)
                ? statusOut : null;

            var orders = await _orderRepository.SearchOrdersAsync(orderId, buyerId, orderStatus, createdOnFrom, createdOnTo);
            return await ProcessOrdersListAsync(orders);
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

                Order newOrder = await CreateNewOrderAsync(orderRequestDto);
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
        /// Process orders in parallel
        /// </summary>
        /// <param name="orders">List of orders.</param>
        /// <returns>List of orders dto.</returns>
        private async Task<List<OrderDto>> ProcessOrdersListAsync(List<Order> orders)
        {
            var ordersListTasks = orders.Select(async order =>
            {
                var (buyerDto, orderProductsDto) = await _orderExternalDataService.GetBuyerDtoAndOrderProductsDtoFromOrderAsync(order);

                OrderDto orderDto = _mapper.Map<OrderDto>(order);
                orderDto.Buyer = buyerDto;
                orderDto.Products = orderProductsDto;
                orderDto.Events = new List<EventDto>() { orderDto.Events.Last() };

                return orderDto;
            });

            OrderDto[] ordersDto = await Task.WhenAll(ordersListTasks);
            return ordersDto.ToList();
        }

        /// <summary>
        /// Creates new order object
        /// </summary>
        /// <param name="orderRequestDto">Order request dto.</param>
        /// <returns>Order object.</returns>
        private async Task<Order> CreateNewOrderAsync(OrderRequestDto orderRequestDto)
        {
            Order order = _mapper.Map<Order>(orderRequestDto);
            var (orderId, buyerId, products) = await _orderExternalDataService.GetDataFromOrderRequestDtoAsync(orderRequestDto);

            order.OrderId = orderId;
            order.BuyerId = buyerId;
            order.Products = products;
            order.Events = [_eventValidationService.CreateNewOrderEvent()];

            return order;
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
