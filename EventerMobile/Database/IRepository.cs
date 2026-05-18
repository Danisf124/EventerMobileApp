namespace EventerMobile.Database;

// Базовий інтерфейс для всіх репозиторіїв
// Визначає стандартні CRUD операції
public interface IRepository<T> where T : class, new()
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<int> InsertAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(int id);
}
