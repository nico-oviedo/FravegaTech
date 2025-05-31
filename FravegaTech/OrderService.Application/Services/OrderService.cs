using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderService.Application.Services.Interfaces;
using OrderService.Data.Repositories;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;
using SharedKernel.Exceptions;

namespace OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventValidationService _eventValidationService;
        private readonly IOrderValidationService _orderValidationService;
        private readonly IOrderExternalDataService _orderExternalDataService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IEventValidationService eventValidationService, IOrderValidationService orderValidationService,
            IOrderExternalDataService orderExternalDataService, IMapper mapper, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _eventValidationService = eventValidationService ?? throw new ArgumentNullException(nameof(eventValidationService));
            _orderValidationService = orderValidationService ?? throw new ArgumentNullException(nameof(orderValidationService));
            _orderExternalDataService = orderExternalDataService ?? throw new ArgumentNullException(nameof(orderExternalDataService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<OrderTranslatedDto> GetFullOrderAsync(int orderId)
        {
            try
            {
                _logger.LogInformation($"Trying to get Order with id: {orderId}.");
                Order order = await _orderRepository.GetByOrderIdAsync(orderId);

                if (order is null)
                {
                    _logger.LogError($"Order with id {orderId} was not found.");
                    throw new NotFoundException(nameof(order), $"{GetType().Name}:{nameof(GetFullOrderAsync)}");
                }

                var (buyerDto, orderProductsDtoList) = await _orderExternalDataService.GetBuyerDtoAndOrderProductsDtoFromOrderAsync(order);
                var fullOrder = _mapper.Map<OrderTranslatedDto>(order);
                fullOrder.Buyer = buyerDto;
                fullOrder.Products = orderProductsDtoList;

                _logger.LogInformation($"Successfully get Order with id: {orderId}.");
                return fullOrder;
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, $"Failed to map Order to OrderTranslatedDto. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task<List<OrderDto>> SearchOrdersAsync(int? orderId, string? documentNumber, string? status,
            DateTime? createdOnFrom, DateTime? createdOnTo)
        {
            try
            {
                _logger.LogInformation("Starting to search Orders with given filters.");

                string? buyerId = documentNumber is not null
                    ? await _orderExternalDataService.GetBuyerIdByDocumentNumberAsync(documentNumber)
                    : null;

                OrderStatus? orderStatus = status is not null
                    && Enum.TryParse<OrderStatus>(status, true, out var statusOut)
                    ? statusOut : null;

                var orders = await _orderRepository.SearchOrdersAsync(orderId, buyerId, orderStatus, createdOnFrom, createdOnTo);
                if (orders is null || !orders.Any())
                {
                    _logger.LogError("No Orders were found with given filters.");
                    throw new NotFoundException(nameof(orders), $"{GetType().Name}:{nameof(SearchOrdersAsync)}");
                }

                _logger.LogInformation($"{orders.Count} Orders were found with given filters.");
                return await ProcessOrdersListAsync(orders);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, $"Failed to map Orders to OrdersDto. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task<OrderCreatedDto> AddOrderAsync(OrderRequestDto orderRequestDto)
        {
            try
            {
                _logger.LogInformation("Trying to add new Order.");

                if (!await _orderValidationService.IsOrderValidAsync(orderRequestDto))
                {
                    _logger.LogError("Invalid Order.");
                    throw new BusinessValidationException("Order", $"{GetType().Name}:{nameof(AddOrderAsync)}");
                }

                Order newOrder = await CreateNewOrderAsync(orderRequestDto);
                string addedOrderId = await _orderRepository.AddOrderAsync(newOrder);

                _logger.LogInformation($"Successfully added new Order with id {addedOrderId}.");
                return _mapper.Map<OrderCreatedDto>(newOrder);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, $"Failed to map Order to OrderCreatedDto. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task<EventAddedDto> AddEventToOrderAsync(int orderId, EventDto eventDto)
        {
            try
            {
                _logger.LogInformation($"Trying to add new Event to Order with id {orderId}.");

                Order order = await _orderRepository.GetByOrderIdAsync(orderId);
                if (order is null)
                {
                    _logger.LogError($"Order with id {orderId} does not exist.");
                    throw new NotFoundException(nameof(order), $"{GetType().Name}:{nameof(AddEventToOrderAsync)}");
                }

                var (isEventValid, isEventNotProcessed) = _eventValidationService.IsEventValidAndNotProcessedAsync(order, eventDto);
                if (isEventValid && isEventNotProcessed)
                    return await ProcessNewEventAsync(orderId, eventDto, order.Status);

                if (!isEventValid)
                {
                    _logger.LogError("Invalid Event.");
                    throw new BusinessValidationException("Event", $"{GetType().Name}:{nameof(AddEventToOrderAsync)}");
                }
                else
                    return _eventValidationService.CreateEventAddedDto(orderId, order.Status.ToString(), eventDto.Type);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, $"Failed to map EventDto to Event. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Process orders in parallel
        /// </summary>
        /// <param name="orders">List of orders.</param>
        /// <returns>List of orders dto.</returns>
        private async Task<List<OrderDto>> ProcessOrdersListAsync(List<Order> orders)
        {
            _logger.LogInformation("Starting to process Orders list.");

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
            _logger.LogInformation("Successfully Orders list was processed.");
            return ordersDto.ToList();
        }

        /// <summary>
        /// Creates new order object
        /// </summary>
        /// <param name="orderRequestDto">Order request dto.</param>
        /// <returns>Order object.</returns>
        private async Task<Order> CreateNewOrderAsync(OrderRequestDto orderRequestDto)
        {
            _logger.LogInformation("Creating Order object to add.");

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
        /// <param name="previousStatus">Order previous status.</param>
        /// <returns>Event added dto object.</returns>
        private async Task<EventAddedDto> ProcessNewEventAsync(int orderId, EventDto eventDto, OrderStatus previousStatus)
        {
            _logger.LogInformation("Starting to process new Event.");

            Event newEvent = _mapper.Map<Event>(eventDto);
            bool wasEventAdded = await _orderRepository.AddEventAsync(orderId, newEvent);

            if (!wasEventAdded)
            {
                string errorMsg = $"Failed to add new Event to Order with id {orderId}.";
                _logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }

            _logger.LogInformation("Updating Order status.");
            await _orderRepository.UpdateOrderStatusAsync(orderId, newEvent.Type);

            _logger.LogInformation("Successfully added new Event.");
            return _eventValidationService.CreateEventAddedDto(orderId, previousStatus.ToString(), newEvent.Type.ToString());
        }
    }
}
