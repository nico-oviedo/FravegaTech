using CounterService.Services;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<OrderExternalDataService> _logger;

        public OrderExternalDataService(ICounterService counterService, BuyerServiceClient buyerServiceClient,
            ProductServiceClient productServiceClient, ILogger<OrderExternalDataService> logger)
        {
            _counterService = counterService ?? throw new ArgumentNullException(nameof(counterService));
            _buyerServiceClient = buyerServiceClient ?? throw new ArgumentNullException(nameof(buyerServiceClient));
            _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<string?> GetBuyerIdByDocumentNumberAsync(string documentNumber)
        {
            _logger.LogInformation($"Trying to get BuyerId with document number: {documentNumber}.");
            return await _buyerServiceClient.GetBuyerIdByDocumentNumberAsync(documentNumber);
        }

        /// <inheritdoc/>
        public async Task<(BuyerDto, List<OrderProductDto>)> GetBuyerDtoAndOrderProductsDtoFromOrderAsync(Order order)
        {
            try
            {
                _logger.LogInformation($"Trying to get BuyerDto and OrderProductsDto from Order with id {order.OrderId}.");

                Task<BuyerDto?> buyerDtoTask = _buyerServiceClient.GetBuyerByIdAsync(order.BuyerId);
                Task<List<OrderProductDto>> orderProductsDtoTask = GetOrderProductsDtoListAsync(order.Products);

                await Task.WhenAll(buyerDtoTask, orderProductsDtoTask);

                BuyerDto? buyerDto = await buyerDtoTask;
                List<OrderProductDto> orderProductsDto = await orderProductsDtoTask;

                _logger.LogInformation($"Successfully get BuyerDto and OrderProductsDto from Order with id {order.OrderId}.");
                return (buyerDto!, orderProductsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get BuyerDto and OrderProductsDto from Order with id {order.OrderId}. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task<(int, string, List<OrderProduct>)> GetDataFromOrderRequestDtoAsync(OrderRequestDto orderRequestDto)
        {
            try
            {
                _logger.LogInformation("Trying to get OrderId, BuyerId and OrderProducts list.");

                Task<int> orderIdTask = _counterService.GetNextSequenceValueAsync(nameof(Order.OrderId));
                Task<string?> buyerIdTask = _buyerServiceClient.AddBuyerAsync(orderRequestDto.Buyer);
                Task<List<OrderProduct>> orderProductsTask = GetOrderProductsListAsync(orderRequestDto.Products);

                await Task.WhenAll(orderIdTask, buyerIdTask, orderProductsTask);

                int orderId = await orderIdTask;
                string? buyerId = await buyerIdTask;
                List<OrderProduct> products = await orderProductsTask;

                _logger.LogInformation("Successfully get OrderId, BuyerId and OrderProducts list.");
                return (orderId, buyerId!, products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get OrderId, BuyerId and OrderProducts list. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Process in parallel order products and gets order products dto list
        /// </summary>
        /// <param name="orderProducts">List of order products.</param>
        /// <returns>List of order products dto objects.</returns>
        private async Task<List<OrderProductDto>> GetOrderProductsDtoListAsync(List<OrderProduct> orderProducts)
        {
            _logger.LogInformation("Trying to get OrderProductsDto list.");

            var productTasks = orderProducts.Select(async product =>
            {
                ProductDto? productDto = await _productServiceClient.GetProductByIdAsync(product.ProductId);
                return new OrderProductDto()
                {
                    SKU = productDto!.SKU,
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Quantity = product.Quantity
                };
            });

            OrderProductDto[] orderProductsDto = await Task.WhenAll(productTasks);
            _logger.LogInformation("Successfully get OrderProductsDto list.");
            return orderProductsDto.ToList();
        }

        /// <summary>
        /// Process in parallel order products dto and gets order products list
        /// </summary>
        /// <param name="orderProductsDto">List of order products dto.</param>
        /// <returns>List of order products objects.></returns>
        private async Task<List<OrderProduct>> GetOrderProductsListAsync(List<OrderProductDto> orderProductsDto)
        {
            _logger.LogInformation("Trying to get OrderProducts list.");

            var productTasks = orderProductsDto.Select(async product =>
            {
                string? productId = await _productServiceClient.AddProductAsync(product);
                return new OrderProduct() { ProductId = productId!, Quantity = product.Quantity };
            });

            OrderProduct[] orderProducts = await Task.WhenAll(productTasks);
            _logger.LogInformation("Successfully get OrderProducts list.");
            return orderProducts.ToList();
        }
    }
}
