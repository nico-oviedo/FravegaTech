using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CounterService.Models
{
    public class Counter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("sequenceName")]
        public string SequenceName { get; set; }

        [BsonElement("sequenceValue")]
        public int SequenceValue { get; set; }
    }
}
