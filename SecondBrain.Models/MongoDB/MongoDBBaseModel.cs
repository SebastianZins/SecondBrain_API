using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SecondBrain.Models.MongoDB
{
    public class MongoDBBaseModel
    {
        [BsonId]
        public ObjectId InternalId { get; set; }

        public Guid Id { get; set; }
    }
}
