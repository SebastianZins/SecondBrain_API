using MongoDB.Bson;
using MongoDB.Driver;
using SecondBrain.Database.MongoDB;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;

namespace SecondBrain.Repositories.MongoDB
{
    public class TextSectionRepository
    {
        private readonly IMongoCollection<TextSectionModel> _collection;

        public TextSectionRepository(FileSectionContext context)
        {
            _collection = context.Text;
        }

        public async Task<TextSectionModel> GetByStructureIdAsync(Guid id)
        {
            try
            {
                return await _collection
                                .Find(section => section.structureId == id)
                                .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Loading text section data failed", e);
            }
        }

        public async Task<List<TextSectionModel>> GetByStructureIdsAsync(List<Guid> ids)
        {
            try
            {
                return await _collection
                                .Find(section => ids.Contains(section.structureId))
                                .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Loading text section data failed", e);
            }
        }

        public async Task CreateAsync(TextSectionModel item)
        {
            try
            {
                await _collection.InsertOneAsync(item);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Creating text section data failed", e);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                DeleteResult actionResult = await _collection.DeleteOneAsync(
                     Builders<TextSectionModel>.Filter.Eq("structureId", id));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Deleting text section data failed", e);
            }
        }

        public async Task<bool> DeleteByIdListAsync(List<Guid> ids)
        {
            try
            {
                DeleteResult actionResult = await _collection.DeleteOneAsync(
                     Builders<TextSectionModel>.Filter.In("structureId", ids));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Deleting text sections data failed", e);
            }
        }

        public async Task<bool> UpdateAsync(Guid id, string text)
        {
            var filter = Builders<TextSectionModel>.Filter.Eq(s => s.structureId, id);
            var update = Builders<TextSectionModel>.Update.Set(s => s.text, text);

            try
            {
                UpdateResult actionResult = await _collection.UpdateOneAsync(filter, update);
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Updating text section data failed", e);
            }
        }

        public async Task<string> CreateIndexAsync()
        {
            try
            {
                IndexKeysDefinition<TextSectionModel> keys = Builders<TextSectionModel>
                                                                .IndexKeys
                                                                .Ascending(item => item.structureId);

                return await _collection.Indexes.CreateOneAsync(new CreateIndexModel<TextSectionModel>(keys));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Creating index failed", e);
            }
        }
    }
}
