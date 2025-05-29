using AutoMapper;
using CounterService.Services;
using OrderService.Application.Services.Interfaces;
using OrderService.Domain;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.ServiceClients;

namespace OrderService.Application.Services
{
    public class OrderExternalDataService : IOrderExternalDataService
    {
        private readonly ICounterService _counterService;
        private readonly BuyerServiceClient _buyerServiceClient;
        private readonly ProductServiceClient _productServiceClient;
        private readonly IMapper _mapper;

        public OrderExternalDataService(ICounterService counterService, BuyerServiceClient buyerServiceClient,
            ProductServiceClient productServiceClient, IMapper mapper)
        {
            _counterService = counterService ?? throw new ArgumentNullException(nameof(counterService));
            _buyerServiceClient = buyerServiceClient ?? throw new ArgumentNullException(nameof(buyerServiceClient));
            _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber)
        {
            return await _buyerServiceClient.GetBuyerIdByDocumentNumberAsync(documentNumber);
        }

        /// <inheritdoc/>
        public async Task<(BuyerDto?, List<OrderProductDto>)> GetBuyerDtoAndOrderProductsDtoFromOrderAsync(Order order)
        {
            Task<BuyerDto?> buyerDtoTask = _buyerServiceClient.GetBuyerByIdAsync(order.BuyerId);
            Task<List<OrderProductDto>> orderProductsDtoTask = GetOrderProductsDtoListAsync(order.Products);

            await Task.WhenAll(buyerDtoTask, orderProductsDtoTask);

            BuyerDto? buyerDto = await buyerDtoTask;
            List<OrderProductDto> orderProductsDto = await orderProductsDtoTask;

            return (buyerDto, orderProductsDto);
        }

        /// <inheritdoc/>
        public async Task<(int, string?, List<OrderProduct>)> GetDataFromOrderRequestDtoAsync(OrderRequestDto orderRequestDto)
        {
            Task<int> orderIdTask = _counterService.GetNextSequenceValueAsync(nameof(Order.OrderId));
            Task<string?> buyerIdTask = _buyerServiceClient.AddBuyerAsync(orderRequestDto.Buyer);
            Task<List<OrderProduct>> orderProductsTask = GetOrderProductsListAsync(orderRequestDto.Products);

            await Task.WhenAll(orderIdTask, buyerIdTask, orderProductsTask);

            int orderId = await orderIdTask;
            string? buyerId = await buyerIdTask;
            List<OrderProduct> products = await orderProductsTask;

            return (orderId, buyerId, products);
        }

        /// <summary>
        /// Process in parallel order products and gets order products dto list
        /// </summary>
        /// <param name="orderProducts">List of order products.</param>
        /// <returns>List of order products dto objects.</returns>
        private async Task<List<OrderProductDto>> GetOrderProductsDtoListAsync(List<OrderProduct> orderProducts)
        {
            var productTasks = orderProducts.Select(async product =>
            {
                ProductDto? productDto = await _productServiceClient.GetProductByIdAsync(product.ProductId);
                return new OrderProductDto()
                {
                    SKU = productDto.SKU,
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Quantity = product.Quantity
                };
            });

            OrderProductDto[] orderProductsDto = await Task.WhenAll(productTasks);
            return orderProductsDto.ToList();
        }

        /// <summary>
        /// Process in parallel order products dto and gets order products list
        /// </summary>
        /// <param name="orderProductsDto">List of order products dto.</param>
        /// <returns>List of order products objects.></returns>
        private async Task<List<OrderProduct>> GetOrderProductsListAsync(List<OrderProductDto> orderProductsDto)
        {
            var productTasks = orderProductsDto.Select(async product =>
            {
                string? productId = await _productServiceClient.AddProductAsync(product);
                return new OrderProduct() { ProductId = productId, Quantity = product.Quantity };
            });

            OrderProduct[] orderProducts = await Task.WhenAll(productTasks);
            return orderProducts.ToList();
        }
    }
}
