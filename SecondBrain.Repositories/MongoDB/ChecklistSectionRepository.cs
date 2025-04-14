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
            try
            {
                return await _collection.Find(_ => true).ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Loading check list section data failed", e);
            }
        }

        public async Task<ChecklistSectionModel> GetByStructureIdAsync(Guid id)
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
                throw new MongoException("Error: Loading check list section data failed", e);
            }
        }

        public async Task<List<ChecklistSectionModel>> GetByStructureIdsAsync(List<Guid> ids)
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
                throw new MongoException("Error: Loading check list section data failed", e);
            }
        }

        public async Task CreateAsync(ChecklistSectionModel item)
        {
            try
            {
                await _collection.InsertOneAsync(item);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Creating check list section data failed", e);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                DeleteResult actionResult = await _collection.DeleteOneAsync(
                     Builders<ChecklistSectionModel>.Filter.Eq("structureId", id));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Deleting check list section data failed", e);
            }
        }

        public async Task<bool> DeleteByIdListAsync(List<Guid> ids)
        {
            try
            {
                DeleteResult actionResult = await _collection.DeleteOneAsync(
                     Builders<ChecklistSectionModel>.Filter.In("structureId", ids));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Deleting check list sections data failed", e);
            }
        }

        public async Task<bool> UpdateAsync(Guid id, List<ChecklistItemModel> items)
        {
            var filter = Builders<ChecklistSectionModel>.Filter.Eq(s => s.structureId, id);
            var update = Builders<ChecklistSectionModel>.Update.Set(s => s.items, items);

            try
            {
                UpdateResult actionResult = await _collection.UpdateOneAsync(filter, update);
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Updating check list section data failed", e);
            }
        }
    }
}
