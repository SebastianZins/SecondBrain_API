using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.MongoDB.Attachment;

namespace SecondBrain.Database.MongoDB
{
    public class AttachmentsContext
    {
        private readonly IMongoDatabase _database;

        public AttachmentsContext(IOptions<MongoDbSettingsModel> settings)
        {
            var mongoDbSettings = settings.Value;
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            _database = client.GetDatabase(mongoDbSettings.FileDatabaseName);
        }

        public IMongoCollection<AttachmentsModel> Attachments
        {
            get
            {
                return _database.GetCollection<AttachmentsModel>("attachments");
            }
        }
    }
}
