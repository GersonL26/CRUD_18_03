using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Application.Mapping;
using CRUD_18_03.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Infrastructure.Services;

public class GenericService : IGenericService
{
    private readonly DbContext _dbContext;
    private readonly IEntityMapper _mapper;

    public GenericService(DbContext dbContext, IEntityMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<object>> GetAllAsync(Type entityType, Type responseDtoType)
    {
        var entities = await GetDbSet(entityType).AsNoTracking().ToListAsync();

        var dtos = new List<object>();
        foreach (var entity in entities)
        {
            var dto = _mapper.MapToNew(entity, responseDtoType);
            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<object?> GetByIdAsync(Type entityType, Type responseDtoType, Guid id)
    {
        var entity = await _dbContext.FindAsync(entityType, id);

        if (entity is null)
            return null;

        return _mapper.MapToNew(entity, responseDtoType);
    }

    public async Task<object> CreateAsync(Type entityType, Type responseDtoType, object createDto)
    {
        var entity = _mapper.MapToNew(createDto, entityType);

        if (entity is BaseEntity baseEntity && baseEntity.Id == Guid.Empty)
        {
            baseEntity.Id = Guid.NewGuid();
        }

        await _dbContext.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return _mapper.MapToNew(entity, responseDtoType);
    }

    public async Task UpdateAsync(Type entityType, Guid id, object updateDto)
    {
        var entity = await _dbContext.FindAsync(entityType, id)
            ?? throw new KeyNotFoundException(
                $"No se encontró la entidad con ID '{id}'.");

        _mapper.MapToExisting(updateDto, entity);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Type entityType, Guid id)
    {
        var entity = await _dbContext.FindAsync(entityType, id);

        if (entity is null)
            return false;

        _dbContext.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private IQueryable<object> GetDbSet(Type entityType)
    {
        var method = typeof(DbContext)
            .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
            .MakeGenericMethod(entityType);

        var dbSet = method.Invoke(_dbContext, null)!;

        var queryableMethod = typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == nameof(Queryable.Cast) && m.GetParameters().Length == 1)
            .MakeGenericMethod(typeof(object));

        return (IQueryable<object>)queryableMethod.Invoke(null, [dbSet])!;
    }
}
