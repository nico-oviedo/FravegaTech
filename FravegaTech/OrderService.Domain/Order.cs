using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrderService.Domain.Enums;

namespace OrderService.Domain
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("orderId")]
        public int OrderId { get; set; }

        [BsonElement("externalReferenceId")]
        public string ExternalReferenceId { get; set; }

        [BsonElement("channel")]
        public SourceChannel Channel { get; set; }

        [BsonElement("purchaseDate")]
        public DateTime PurchaseDate { get; set; }

        [BsonElement("totalValue")]
        public decimal TotalValue { get; set; }

        [BsonElement("buyerId")]
        public string BuyerId { get; set; }

        [BsonElement("orderProducts")]
        public IEnumerable<OrderProduct> OrderProducts { get; set; }

        [BsonElement("status")]
        public OrderStatus Status { get; set; }

        [BsonElement("events")]
        public IEnumerable<Event> Events { get; set; }
    }
}
