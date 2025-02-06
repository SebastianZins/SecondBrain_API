using MongoDB.Bson;
using MongoDB.Driver;
using SecondBrain.Core.Interfaces;
using SecondBrain.Database.MongoDB;
using SecondBrain.Models.MongoDB;

namespace SecondBrain.Repositories.MongoDB
{
    public class FileRepository : IMongoDBRepository<FileModel>
    {
        private readonly FilesContext _context;

        public FileRepository(string connectionString, string database)
        {
            _context = new FilesContext(connectionString, database);
        }

        public async Task<IEnumerable<FileModel>> GetAll()
        {
            try
            {
                return await _context.Files.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<FileModel> Get(Guid id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                return await _context.Files
                                .Find(note => note.Id == id || note.InternalId == internalId)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task Create(FileModel item)
        {
            try
            {
                await _context.Files.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                DeleteResult actionResult = await _context.Files.DeleteOneAsync(
                     Builders<FileModel>.Filter.Eq("Id", id));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> Update(Guid id, FileModel body)
        {
            var filter = Builders<FileModel>.Filter.Eq(s => s.Id, id);
            //var update = Builders<FileModel>.Update
            //                .Set(s => s.Body, body)
            //                .CurrentDate(s => s.UpdatedOn);
            // TODO:

            try
            {
               // UpdateResult actionResult = await _context.Files.UpdateOneAsync(filter, update);

                //return actionResult.IsAcknowledged
                //    && actionResult.ModifiedCount > 0;

                return false;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        private ObjectId GetInternalId(Guid id)
        {
            if (!ObjectId.TryParse(id.ToString(), out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task<string> CreateIndex()
        {
            try
            {
                IndexKeysDefinition<FileModel> keys = Builders<FileModel>
                                                    .IndexKeys
                                                    .Ascending(item => item.Id);

                return await _context.Files
                                .Indexes.CreateOneAsync(new CreateIndexModel<FileModel>(keys));
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}
