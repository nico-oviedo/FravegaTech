using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Exceptions;

namespace OrderService.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly TimeZoneInfo _timeZoneArg;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(IConfiguration config, ILogger<OrderRepository> logger)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config.GetConnectionString("OrderDatabase"));
            _orders = database.GetCollection<Order>(config.GetConnectionString("OrdersCollection"));
            _timeZoneArg = TimeZoneInfo.FindSystemTimeZoneById(config.GetSection("TimeZones")["TimeZoneARG"]!);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<Order> GetByOrderIdAsync(int orderId)
        {
            try
            {
                return await _orders.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get Order by id {orderId} from database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(GetByOrderIdAsync)}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsUniqueExternalReferenceInChannelAsync(string externalReferenceId, SourceChannel channel)
        {
            try
            {
                Order order = await _orders
                    .Find(o => o.ExternalReferenceId.ToLower() == externalReferenceId.ToLower() && o.Channel == channel)
                    .FirstOrDefaultAsync();

                return order is null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get Order external reference {externalReferenceId} in channel {channel.ToString()} from database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(IsUniqueExternalReferenceInChannelAsync)}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> AddOrderAsync(Order order)
        {
            try
            {
                order.PurchaseDate = order.PurchaseDate.Kind != DateTimeKind.Utc
                    ? TimeZoneInfo.ConvertTimeToUtc(order.PurchaseDate, _timeZoneArg)
                    : order.PurchaseDate;

                await _orders.InsertOneAsync(order);
                return order._id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to insert new Order in database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(AddOrderAsync)}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> AddEventAsync(int orderId, Event newEvent)
        {
            try
            {
                newEvent.Date = newEvent.Date.Kind != DateTimeKind.Utc
                    ? TimeZoneInfo.ConvertTimeToUtc(newEvent.Date, _timeZoneArg)
                    : newEvent.Date;

                var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
                var update = Builders<Order>.Update.Push(o => o.Events, newEvent);
                var result = await _orders.UpdateOneAsync(filter, update);

                return result.MatchedCount == 1 && result.ModifiedCount == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to insert new Event for Order with id {orderId} in database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(AddEventAsync)}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
                var update = Builders<Order>.Update.Set(o => o.Status, newStatus);
                var result = await _orders.UpdateOneAsync(filter, update);

                return result.MatchedCount == 1 && result.ModifiedCount == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update OrderStatus for Order with id {orderId} in database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(UpdateOrderStatusAsync)}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<List<Order>> SearchOrdersAsync(int? orderId, string? buyerId, OrderStatus? status,
            DateTime? createdOnFrom, DateTime? createdOnTo)
        {
            try
            {
                var builder = Builders<Order>.Filter;
                var finalFilter = builder.And(BuildORFiltersForSearchOrders(orderId, buyerId, status),
                    BuildANDFiltersForSearchOrders(createdOnFrom, createdOnTo));

                return await _orders
                    .Find(finalFilter)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to search Orders with given filters from database. {ex.Message}");
                throw new DataAccessException($"{GetType().Name}:{nameof(SearchOrdersAsync)}", ex);
            }
        }

        /// <summary>
        /// Builds "Or" filters for search orders
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="buyerId">Order buyer id.</param>
        /// <param name="status">Order status.</param>
        /// <returns>Definition with "Or" filters.</returns>
        private FilterDefinition<Order> BuildORFiltersForSearchOrders(int? orderId, string? buyerId, OrderStatus? status)
        {
            var builder = Builders<Order>.Filter;
            var filtersOr = new List<FilterDefinition<Order>>();

            if (orderId is not null)
                filtersOr.Add(builder.Eq(o => o.OrderId, orderId));

            if (!string.IsNullOrEmpty(buyerId))
                filtersOr.Add(builder.Eq(o => o.BuyerId, buyerId));

            if (status is not null)
                filtersOr.Add(builder.Eq(o => o.Status, status));

            return filtersOr.Any() ? builder.Or(filtersOr) : builder.Empty;
        }

        /// <summary>
        /// Builds "And" filters for search orders
        /// </summary>
        /// <param name="createdOnFrom">Order created from.</param>
        /// <param name="createdOnTo">Order created to.</param>
        /// <returns>Definition with "And" filters.</returns>
        private FilterDefinition<Order> BuildANDFiltersForSearchOrders(DateTime? createdOnFrom, DateTime? createdOnTo)
        {
            var builder = Builders<Order>.Filter;
            var filtersAnd = new List<FilterDefinition<Order>>();

            if (createdOnFrom.HasValue)
            {
                var utcFrom = TimeZoneInfo.ConvertTimeToUtc(createdOnFrom.Value, _timeZoneArg);
                filtersAnd.Add(builder.Gte(o => o.PurchaseDate, utcFrom));
            }

            if (createdOnTo.HasValue)
            {
                var utcTo = TimeZoneInfo.ConvertTimeToUtc(createdOnTo.Value, _timeZoneArg);
                filtersAnd.Add(builder.Lte(o => o.PurchaseDate, utcTo));
            }

            return filtersAnd.Any() ? builder.And(filtersAnd) : builder.Empty;
        }
    }
}
