using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BuyerService.Domain
{
    public class Buyer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("documentNumber")]
        public string DocumentNumber { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }
    }
}
