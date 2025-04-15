using MongoDB.Driver;
using SecondBrain.Database.MongoDB;
using SecondBrain.Models.DatabaseModels.MongoDB.File;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;

namespace SecondBrain.Repositories.MongoDB
{
    public class ChecklistSectionRepository : MongoDBRepository<ChecklistSectionModel>
    {
        public ChecklistSectionRepository(FileSectionContext context) : base(context.Checklist) { }

        public async Task<List<ChecklistSectionModel>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<ChecklistSectionModel> GetByStructureIdAsync(Guid id)
        {
            return await _collection
                            .Find(section => section.structureId == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<ChecklistSectionModel>> GetByStructureIdsAsync(List<Guid> ids)
        {
            return await _collection
                            .Find(section => ids.Contains(section.structureId))
                            .ToListAsync();
        }

        public async Task CreateAsync(ChecklistSectionModel item)
        {
            await _collection.InsertOneAsync(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            DeleteResult actionResult = await _collection.DeleteOneAsync(
                 Builders<ChecklistSectionModel>.Filter.Eq("structureId", id));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task<bool> DeleteByIdListAsync(List<Guid> ids)
        {
            DeleteResult actionResult = await _collection.DeleteOneAsync(
                 Builders<ChecklistSectionModel>.Filter.In("structureId", ids));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task<bool> UpdateAsync(Guid id, List<ChecklistItemModel> items)
        {
            var filter = Builders<ChecklistSectionModel>.Filter.Eq(s => s.structureId, id);
            var update = Builders<ChecklistSectionModel>.Update.Set(s => s.items, items);

            UpdateResult actionResult = await _collection.UpdateOneAsync(filter, update);
            return actionResult.IsAcknowledged
                && actionResult.ModifiedCount > 0;
        }
    }
}
