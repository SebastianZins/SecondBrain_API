namespace SecondBrain.Core.Interfaces
{
    public interface IMongoDBRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> Get(Guid id);

        Task Create(T item);

        Task<bool> Delete(Guid id);

        Task<bool> Update(Guid id, T item);

        Task<string> CreateIndex();
    }
}
