using MongoDB.Driver;
using SecondBrain.Database.MongoDB;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;

namespace SecondBrain.Repositories.MongoDB
{
    public class ListSectionRepository : MongoDBRepository<ListSectionModel>
    {

        public ListSectionRepository(FileSectionContext context) : base(context.List) { }

        public async Task<List<ListSectionModel>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<ListSectionModel> GetByStructureIdAsync(Guid id)
        {
            return await _collection
                            .Find(section => section.structureId == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<ListSectionModel>> GetByStructureIdsAsync(List<Guid> ids)
        {
            return await _collection
                            .Find(section => ids.Contains(section.structureId))
                            .ToListAsync();
        }

        public async Task CreateAsync(ListSectionModel item)
        {
            await _collection.InsertOneAsync(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            DeleteResult actionResult = await _collection.DeleteOneAsync(
                 Builders<ListSectionModel>.Filter.Eq("structureId", id));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task<bool> DeleteByIdListAsync(List<Guid> ids)
        {
            DeleteResult actionResult = await _collection.DeleteOneAsync(
                 Builders<ListSectionModel>.Filter.In("structureId", ids));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task<bool> UpdateAsync(Guid id, List<string> items)
        {
            var filter = Builders<ListSectionModel>.Filter.Eq(s => s.structureId, id);
            var update = Builders<ListSectionModel>.Update.Set(s => s.items, items);

            UpdateResult actionResult = await _collection.UpdateOneAsync(filter, update);
            return actionResult.IsAcknowledged
                && actionResult.ModifiedCount > 0;
        }
    }
}
