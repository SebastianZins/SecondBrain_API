using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SecondBrain.Models.DatabaseModels.MongoDB
{
    public class MongoDBBaseModel : IMongoDBBaseModel
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid structureId { get; set; } = Guid.Empty;
    }
}
