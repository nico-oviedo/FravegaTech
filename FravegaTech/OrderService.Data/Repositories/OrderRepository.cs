using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using OrderService.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config.GetConnectionString("OrderDatabase"));
            _orders = database.GetCollection<Order>(config.GetConnectionString("OrdersCollection"));
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
        public async Task<List<Order>> SearchOrdersAsync(Dictionary<string, object> filters)
        {
            //Armar query segun los filtros y buscar

            return null;
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
        public async Task<string?> AddOrderAsync(Order order)
        {
            try
            {
                await _orders.InsertOneAsync(order);
                return order._id;
            }
            catch (Exception ex)
            {
                //Loguear exception
                return null;
            }
        }
    }
}
