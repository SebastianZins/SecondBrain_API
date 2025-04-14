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
            return await _collection
                            .Find(section => section.structureId == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<TextSectionModel>> GetByStructureIdsAsync(List<Guid> ids)
        {
            return await _collection
                            .Find(section => ids.Contains(section.structureId))
                            .ToListAsync();
        }

        public async Task CreateAsync(TextSectionModel item)
        {
            await _collection.InsertOneAsync(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            DeleteResult actionResult = await _collection.DeleteOneAsync(
                 Builders<TextSectionModel>.Filter.Eq("structureId", id));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task<bool> DeleteByIdListAsync(List<Guid> ids)
        {
            DeleteResult actionResult = await _collection.DeleteOneAsync(
                 Builders<TextSectionModel>.Filter.In("structureId", ids));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task<bool> UpdateAsync(Guid id, string text)
        {
            var filter = Builders<TextSectionModel>.Filter.Eq(s => s.structureId, id);
            var update = Builders<TextSectionModel>.Update.Set(s => s.text, text);

            UpdateResult actionResult = await _collection.UpdateOneAsync(filter, update);
            return actionResult.IsAcknowledged
                && actionResult.ModifiedCount > 0;
        }

        public async Task<string> CreateIndexAsync()
        {
            IndexKeysDefinition<TextSectionModel> keys = Builders<TextSectionModel>
                                                            .IndexKeys
                                                            .Ascending(item => item.structureId);

            return await _collection.Indexes.CreateOneAsync(new CreateIndexModel<TextSectionModel>(keys));
        }
    }
}
