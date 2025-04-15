using MongoDB.Bson;

namespace SecondBrain.Models.DatabaseModels.MongoDB
{
    public interface IMongoDBBaseModel
    {
        ObjectId _id { get; set; }
        Guid structureId { get; set; }
    }
}