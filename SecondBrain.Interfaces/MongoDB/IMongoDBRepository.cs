
namespace SecondBrain.Repositories.MongoDB
{
    public interface IMongoDBRepository
    {
        Task<string> CreateIndexAsync();
    }
}