using CRUD_18_03.Application.DTOs.Categoria;
using CRUD_18_03.Application.DTOs.Producto;
using CRUD_18_03.Application.DTOs.Proveedor;
using CRUD_18_03.Application.DTOs.Sucursal;
using CRUD_18_03.Application.DTOs.Supervisor;
using CRUD_18_03.Application.DTOs.Ubicacion;
using CRUD_18_03.Domain.Entities;

namespace CRUD_18_03.Application.Metadata;

public class EntityMetadataProvider : IEntityMetadataProvider
{
    private readonly Dictionary<string, EntityMetadata> _registry = new(StringComparer.OrdinalIgnoreCase);

    public EntityMetadataProvider()
    {
        Register("sucursal",   typeof(Sucursal),   typeof(SucursalDto),   typeof(CrearSucursalDto),   typeof(ActualizarSucursalDto));
        Register("supervisor", typeof(Supervisor), typeof(SupervisorDto), typeof(CrearSupervisorDto), typeof(ActualizarSupervisorDto));
        Register("producto",   typeof(Producto),   typeof(ProductoDto),   typeof(CrearProductoDto),   typeof(ActualizarProductoDto));
        Register("categoria",  typeof(Categoria),  typeof(CategoriaDto),  typeof(CrearCategoriaDto),  typeof(ActualizarCategoriaDto));
        Register("proveedor",  typeof(Proveedor),  typeof(ProveedorDto),  typeof(CrearProveedorDto),  typeof(ActualizarProveedorDto));
        Register("ubicacion",  typeof(Ubicacion),  typeof(UbicacionDto),  typeof(CrearUbicacionDto),  typeof(ActualizarUbicacionDto));
    }

    public EntityMetadata? GetMetadata(string entityName)
    {
        _registry.TryGetValue(entityName, out var metadata);
        return metadata;
    }

    public IEnumerable<string> GetRegisteredEntityNames()
    {
        return _registry.Keys;
    }

    private void Register(string entityName, Type entityType, Type responseDtoType, Type createDtoType, Type updateDtoType)
    {
        var metadata = new EntityMetadata(entityName, entityType, responseDtoType, createDtoType, updateDtoType);
        _registry[entityName] = metadata;
    }
}
