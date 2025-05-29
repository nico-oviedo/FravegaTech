using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using OrderService.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly TimeZoneInfo _timeZoneArg;

        public OrderRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config.GetConnectionString("OrderDatabase"));
            _orders = database.GetCollection<Order>(config.GetConnectionString("OrdersCollection"));
            _timeZoneArg = TimeZoneInfo.FindSystemTimeZoneById(config.GetSection("TimeZones")["TimeZoneARG"]!);
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
                //Loguear exception
                return null;
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
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsUniqueEventIdAsync(int orderId, string eventId)
        {
            try
            {
                bool existEventId = await _orders
                    .Find(o => o.OrderId == orderId && o.Events.Any(e => e.EventId.ToLower() == eventId.ToLower()))
                    .AnyAsync();

                return !existEventId;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsEventAlreadyProcessedAsync(int orderId, OrderStatus eventType)
        {
            try
            {
                bool eventProcessed = await _orders
                    .Find(o => o.OrderId == orderId && o.Events.Any(e => e.Type == eventType))
                    .AnyAsync();

                return eventProcessed;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<OrderStatus?> GetOrderStatusAsync(int orderId)
        {
            try
            {
                var result = await _orders
                    .Find(o => o.OrderId == orderId)
                    .Project(o => new { o.Status })
                    .FirstOrDefaultAsync();

                return result?.Status;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<string?> AddOrderAsync(Order order)
        {
            try
            {
                order.PurchaseDate = TimeZoneInfo.ConvertTimeToUtc(order.PurchaseDate, _timeZoneArg);
                await _orders.InsertOneAsync(order);
                return order._id;
            }
            catch (Exception ex)
            {
                //Loguear exception
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> AddEventAsync(int orderId, Event newEvent)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
                var update = Builders<Order>.Update.Push(o => o.Events, newEvent);
                var result = await _orders.UpdateOneAsync(filter, update);

                return result.MatchedCount == 1 && result.ModifiedCount == 1;
            }
            catch (Exception ex)
            {
                return false;
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
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<List<Order>> SearchOrdersAsync(int? orderId, string? buyerId, OrderStatus? status,
            DateTime? createdOnFrom, DateTime? createdOnTo)
        {
            try
            {
                var builder = Builders<Order>.Filter;
                var finalFilter = builder.And(GenerateFiltersOr(orderId, buyerId, status), GenerateFiltersAnd(createdOnFrom, createdOnTo));

                return await _orders
                    .Find(finalFilter)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Generates Filters "Or"
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="buyerId">Order buyer id.</param>
        /// <param name="status">Order status.</param>
        /// <returns>Definition with "Or" filters.</returns>
        private FilterDefinition<Order> GenerateFiltersOr(int? orderId, string? buyerId, OrderStatus? status)
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
        /// Generates Filters "And"
        /// </summary>
        /// <param name="createdOnFrom">Order created from.</param>
        /// <param name="createdOnTo">Order created to.</param>
        /// <returns>Definition with "And" filters.</returns>
        private FilterDefinition<Order> GenerateFiltersAnd(DateTime? createdOnFrom, DateTime? createdOnTo)
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
