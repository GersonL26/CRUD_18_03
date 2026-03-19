using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Application.Mapping;
using CRUD_18_03.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        var query = GetDbSet(entityType);

        // Filtrar solo activos si la entidad tiene EstaActivo
        if (typeof(BaseEntity).IsAssignableFrom(entityType))
            query = ApplyActiveFilter(query, entityType);

        var entities = await query.AsNoTracking().ToListAsync();

        var dtos = new List<object>();
        foreach (var entity in entities)
        {
            var dto = _mapper.MapToNew(entity, responseDtoType);
            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<IEnumerable<object>> GetByFilterAsync(Type entityType, Type responseDtoType, string propertyName, object value)
    {
        var query = GetDbSet(entityType);

        // Filtrar solo activos
        if (typeof(BaseEntity).IsAssignableFrom(entityType))
            query = ApplyActiveFilter(query, entityType);

        // Aplicar filtro dinámico por propiedad
        query = ApplyPropertyFilter(query, entityType, propertyName, value);

        var entities = await query.AsNoTracking().ToListAsync();

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

        // Verificar que esté activo
        if (entity is BaseEntity baseEntity && !baseEntity.EstaActivo)
            return null;

        return _mapper.MapToNew(entity, responseDtoType);
    }

    public async Task<object> CreateAsync(Type entityType, Type responseDtoType, object createDto)
    {
        var entity = _mapper.MapToNew(createDto, entityType);

        if (entity is BaseEntity baseEntity)
        {
            if (baseEntity.Id == Guid.Empty)
                baseEntity.Id = Guid.NewGuid();

            baseEntity.CreadoEn = DateTime.UtcNow;
            baseEntity.EstaActivo = true;
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

        if (entity is BaseEntity baseEntity && !baseEntity.EstaActivo)
            throw new KeyNotFoundException(
                $"No se encontró la entidad con ID '{id}'.");

        _mapper.MapToExisting(updateDto, entity);

        if (entity is BaseEntity be)
            be.ModificadoEn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Type entityType, Guid id)
    {
        var entity = await _dbContext.FindAsync(entityType, id);

        if (entity is null)
            return false;

        // Soft delete si es BaseEntity
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.EstaActivo = false;
            baseEntity.ModificadoEn = DateTime.UtcNow;
        }
        else
        {
            _dbContext.Remove(entity);
        }

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

    private static IQueryable<object> ApplyActiveFilter(IQueryable<object> query, Type entityType)
    {
        // param => ((EntityType)param).EstaActivo == true
        var param = Expression.Parameter(typeof(object), "e");
        var cast = Expression.Convert(param, entityType);
        var property = Expression.Property(cast, nameof(BaseEntity.EstaActivo));
        var condition = Expression.Equal(property, Expression.Constant(true));
        var lambda = Expression.Lambda<Func<object, bool>>(condition, param);

        return query.Where(lambda);
    }

    private static IQueryable<object> ApplyPropertyFilter(IQueryable<object> query, Type entityType, string propertyName, object value)
    {
        var prop = entityType.GetProperty(propertyName)
            ?? throw new ArgumentException($"La propiedad '{propertyName}' no existe en '{entityType.Name}'.");

        // param => ((EntityType)param).Property == value
        var param = Expression.Parameter(typeof(object), "e");
        var cast = Expression.Convert(param, entityType);
        var property = Expression.Property(cast, prop);
        var constant = Expression.Constant(value, prop.PropertyType);
        var condition = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<object, bool>>(condition, param);

        return query.Where(lambda);
    }
}
