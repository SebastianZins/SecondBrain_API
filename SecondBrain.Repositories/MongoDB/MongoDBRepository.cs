using MongoDB.Driver;
using SecondBrain.Models.DatabaseModels.MongoDB;

namespace SecondBrain.Repositories.MongoDB
{
    public class MongoDBRepository<T> : IMongoDBRepository where T : IMongoDBBaseModel
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoDBRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public async Task<string> CreateIndexAsync()
        {
            try
            {
                IndexKeysDefinition<T> keys = Builders<T>
                    .IndexKeys
                    .Ascending(item => item.structureId);

                return await _collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(keys));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Creating index failed", e);
            }
        }
    }
}
