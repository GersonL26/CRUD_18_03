namespace CRUD_18_03.Application.Interfaces;

public interface IGenericService
{
    Task<IEnumerable<object>> GetAllAsync(Type entityType, Type responseDtoType);
    Task<object?> GetByIdAsync(Type entityType, Type responseDtoType, Guid id);
    Task<object> CreateAsync(Type entityType, Type responseDtoType, object createDto);
    Task UpdateAsync(Type entityType, Guid id, object updateDto);
    Task<bool> DeleteAsync(Type entityType, Guid id);
}
