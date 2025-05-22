using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Domain
{
    public class OrderProduct
    {
        [BsonElement("productId")]
        public string ProductId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }
    }
}
