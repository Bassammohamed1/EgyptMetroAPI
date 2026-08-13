namespace MetroAPI.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> Get(int id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        T Update(T entity);
        T Delete(T entity);
    }
}
