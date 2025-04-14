using MongoDB.Driver;
using SecondBrain.Database.MongoDB;
using SecondBrain.Models.DatabaseModels.MongoDB.Attachment;

namespace SecondBrain.Repositories.MongoDB
{
    public class AttachmentRepository : MongoDBRepository<AttachmentsModel>
    {
        public AttachmentRepository(AttachmentsContext context) : base(context.Attachments){}

        public async Task<IEnumerable<AttachmentsModel>> GetAll()
        {
            try
            {
                return await _collection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        //public async Task<AttachmentsModel> Get(Guid id)
        //{
        //    try
        //    {
        //        ObjectId internalId = GetInternalId(id);
        //        return await _context.Attachments
        //                        .Find(note => note.Id == id || note.Id == internalId)
        //                        .FirstOrDefaultAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // log or manage the exception
        //        throw ex;
        //    }
        //}

        //public async Task Create(AttachmentsModel item)
        //{
        //    try
        //    {
        //        await _context.Attachments.InsertOneAsync(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        // log or manage the exception
        //        throw ex;
        //    }
        //}

        //public async Task<bool> Delete(Guid id)
        //{
        //    try
        //    {
        //        DeleteResult actionResult = await _context.Attachments.DeleteOneAsync(
        //             Builders<AttachmentsModel>.Filter.Eq("Id", id));

        //        return actionResult.IsAcknowledged
        //            && actionResult.DeletedCount > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        // log or manage the exception
        //        throw ex;
        //    }
        //}

        //public async Task<bool> Update(Guid id, AttachmentsModel body)
        //{
        //    var filter = Builders<AttachmentsModel>.Filter.Eq(s => s.Id, id);
        //    //var update = Builders<FileModel>.Update
        //    //                .Set(s => s.Body, body)
        //    //                .CurrentDate(s => s.UpdatedOn);
        //    // TODO:

        //    try
        //    {
        //       // UpdateResult actionResult = await _context.Files.UpdateOneAsync(filter, update);

        //        //return actionResult.IsAcknowledged
        //        //    && actionResult.ModifiedCount > 0;

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        // log or manage the exception
        //        throw ex;
        //    }
        //}
        //private ObjectId GetInternalId(Guid id)
        //{
        //    if (!ObjectId.TryParse(id.ToString(), out ObjectId internalId))
        //        internalId = ObjectId.Empty;

        //    return internalId;
        //}

        //public async Task<string> CreateIndex()
        //{
        //    try
        //    {
        //        IndexKeysDefinition<AttachmentsModel> keys = Builders<AttachmentsModel>
        //                                            .IndexKeys
        //                                            .Ascending(item => item.Id);

        //        return await _context.Attachments
        //                        .Indexes.CreateOneAsync(new CreateIndexModel<AttachmentsModel>(keys));
        //    }
        //    catch (Exception ex)
        //    {
        //        // log or manage the exception
        //        throw ex;
        //    }
        //}
    }
}
