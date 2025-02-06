using MongoDB.Driver;
using SecondBrain.Models.DatabaseModels.MongoDB;

namespace SecondBrain.Database.MongoDB
{
    public class FilesContext
    {
        private readonly IMongoDatabase _database;

        public FilesContext(string connectionString, string database)
        {
            var client = new MongoClient(connectionString);
            if (client != null) {
                _database = client.GetDatabase(database);
            }
        }

        public IMongoCollection<FileModel> Files
        {
            get
            {
                return _database.GetCollection<FileModel>("SecondBrain");
            }
        }
    }
}
