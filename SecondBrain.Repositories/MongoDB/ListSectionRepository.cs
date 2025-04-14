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
            try
            {
                return await _collection.Find(_ => true).ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Loading list section data failed", e);
            }
        }

        public async Task<ListSectionModel> GetByStructureIdAsync(Guid id)
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
                throw new MongoException("Error: Loading list section data failed", e);
            }
        }

        public async Task<List<ListSectionModel>> GetByStructureIdsAsync(List<Guid> ids)
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
                throw new MongoException("Error: Loading list section data failed", e);
            }
        }

        public async Task CreateAsync(ListSectionModel item)
        {
            try
            {
                await _collection.InsertOneAsync(item);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Creating list section data failed", e);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                DeleteResult actionResult = await _collection.DeleteOneAsync(
                     Builders<ListSectionModel>.Filter.Eq("structureId", id));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Deleting list section data failed", e);
            }
        }

        public async Task<bool> DeleteByIdListAsync(List<Guid> ids)
        {
            try
            {
                DeleteResult actionResult = await _collection.DeleteOneAsync(
                     Builders<ListSectionModel>.Filter.In("structureId", ids));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Deleting list sections data failed", e);
            }
        }

        public async Task<bool> UpdateAsync(Guid id, List<string> items)
        {
            var filter = Builders<ListSectionModel>.Filter.Eq(s => s.structureId, id);
            var update = Builders<ListSectionModel>.Update.Set(s => s.items, items);

            try
            {
                UpdateResult actionResult = await _collection.UpdateOneAsync(filter, update);
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new MongoException("Error: Updating list section data failed", e);
            }
        }
    }
}
