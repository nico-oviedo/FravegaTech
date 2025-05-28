using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrderService.Domain.Enums;

namespace OrderService.Domain
{
    public class Event
    {
        [BsonElement("eventId")]
        public string EventId { get; set; }

        [BsonElement("type")]
        public OrderStatus Type { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("user")]
        public string User { get; set; }
    }
}
