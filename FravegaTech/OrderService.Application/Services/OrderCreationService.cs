using AutoMapper;
using CounterService.Services;
using OrderService.Application.Services.Interfaces;
using OrderService.Domain;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.ServiceClients;

namespace OrderService.Application.Services
{
    public class OrderCreationService : IOrderCreationService
    {
        private readonly ICounterService _counterService;
        private readonly IEventService _eventService;
        private readonly BuyerServiceClient _buyerServiceClient;
        private readonly ProductServiceClient _productServiceClient;
        private readonly IMapper _mapper;

        public OrderCreationService(ICounterService counterService, IEventService eventService, BuyerServiceClient buyerServiceClient,
            ProductServiceClient productServiceClient, IMapper mapper)
        {
            _counterService = counterService ?? throw new ArgumentNullException(nameof(counterService));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _buyerServiceClient = buyerServiceClient ?? throw new ArgumentNullException(nameof(buyerServiceClient));
            _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<Order> CreateNewOrderAsync(OrderRequestDto orderRequestDto)
        {
            Order order = _mapper.Map<Order>(orderRequestDto);
            var (orderId, buyerId, products) = await GetOrderExternalData(orderRequestDto);

            order.OrderId = orderId;
            order.BuyerId = buyerId;
            order.Products = products;
            order.Events = [_eventService.CreateNewOrderEvent()];

            return order;
        }

        /// <summary>
        /// Gets order external data
        /// </summary>
        /// <param name="orderRequestDto">Order request dto.</param>
        /// <returns>Tuple with order id, buyer id and list of order product.</returns>
        private async Task<(int, string?, List<OrderProduct>)> GetOrderExternalData(OrderRequestDto orderRequestDto)
        {
            Task<int> orderIdTask = _counterService.GetNextSequenceValueAsync(nameof(Order.OrderId));
            Task<string?> buyerIdTask = _buyerServiceClient.AddBuyerAsync(orderRequestDto.Buyer);
            Task<List<OrderProduct>> orderProductsTask = GetOrderProductListAsync(orderRequestDto.Products);

            await Task.WhenAll(orderIdTask, buyerIdTask, orderProductsTask);

            int orderId = await orderIdTask;
            string? buyerId = await buyerIdTask;
            List<OrderProduct> products = await orderProductsTask;

            return (orderId, buyerId, products);
        }

        /// <summary>
        /// Gets order product list
        /// </summary>
        /// <param name="orderProductsDto">List of order products dto.</param>
        /// <returns>List of order product object.></returns>
        private async Task<List<OrderProduct>> GetOrderProductListAsync(List<OrderProductDto> orderProductsDto)
        {
            var productTasks = orderProductsDto.Select(async product =>
            {
                var productId = await _productServiceClient.AddProductAsync(product);
                return new OrderProduct() { ProductId = productId, Quantity = product.Quantity };
            });

            OrderProduct[] orderProducts = await Task.WhenAll(productTasks);
            return orderProducts.ToList();
        }
    }
}
