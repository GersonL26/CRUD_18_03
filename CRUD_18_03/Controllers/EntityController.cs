using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Application.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/entity/{entityName}")]
[Authorize]
public class EntityController : ControllerBase
{
    private readonly IGenericService _service;
    private readonly IEntityMetadataProvider _metadataProvider;

    public EntityController(IGenericService service, IEntityMetadataProvider metadataProvider)
    {
        _service = service;
        _metadataProvider = metadataProvider;
    }

    [HttpGet("~/api/entity")]
    public IActionResult GetCatalog()
    {
        var catalog = _metadataProvider.GetRegisteredEntityNames()
            .OrderBy(n => n)
            .Select(name =>
            {
                var metadata = _metadataProvider.GetMetadata(name)!;
                return new
                {
                    entidad = name,
                    crear = BuildDtoSchema(metadata.CreateDtoType),
                    actualizar = BuildDtoSchema(metadata.UpdateDtoType),
                    respuesta = BuildDtoSchema(metadata.ResponseDtoType)
                };
            });

        return Ok(catalog);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(string entityName)
    {
        var metadata = _metadataProvider.GetMetadata(entityName);
        if (metadata is null)
            return NotFound(new { message = $"Entidad '{entityName}' no registrada." });

        var result = await _service.GetAllAsync(metadata.EntityType, metadata.ResponseDtoType);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(string entityName, Guid id)
    {
        var metadata = _metadataProvider.GetMetadata(entityName);
        if (metadata is null)
            return NotFound(new { message = $"Entidad '{entityName}' no registrada." });

        var result = await _service.GetByIdAsync(metadata.EntityType, metadata.ResponseDtoType, id);
        if (result is null)
            return NotFound(new { message = $"No se encontró {entityName} con ID '{id}'." });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string entityName, [FromBody] JsonElement body)
    {
        var metadata = _metadataProvider.GetMetadata(entityName);
        if (metadata is null)
            return NotFound(new { message = $"Entidad '{entityName}' no registrada." });

        var dto = JsonSerializer.Deserialize(body.GetRawText(), metadata.CreateDtoType,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (dto is null)
            return BadRequest(new { message = "El cuerpo de la solicitud no es válido." });

        var result = await _service.CreateAsync(metadata.EntityType, metadata.ResponseDtoType, dto);
        return StatusCode(201, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(string entityName, Guid id, [FromBody] JsonElement body)
    {
        var metadata = _metadataProvider.GetMetadata(entityName);
        if (metadata is null)
            return NotFound(new { message = $"Entidad '{entityName}' no registrada." });

        var dto = JsonSerializer.Deserialize(body.GetRawText(), metadata.UpdateDtoType,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (dto is null)
            return BadRequest(new { message = "El cuerpo de la solicitud no es válido." });

        await _service.UpdateAsync(metadata.EntityType, id, dto);
        return Ok(new { message = $"{entityName} actualizado correctamente." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(string entityName, Guid id)
    {
        var metadata = _metadataProvider.GetMetadata(entityName);
        if (metadata is null)
            return NotFound(new { message = $"Entidad '{entityName}' no registrada." });

        var deleted = await _service.DeleteAsync(metadata.EntityType, id);
        if (!deleted)
            return NotFound(new { message = $"No se encontró {entityName} con ID '{id}'." });

        return Ok(new { message = $"{entityName} eliminado correctamente." });
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetByFilter(string entityName, [FromQuery] string property, [FromQuery] string value)
    {
        var metadata = _metadataProvider.GetMetadata(entityName);
        if (metadata is null)
            return NotFound(new { message = $"Entidad '{entityName}' no registrada." });

        var prop = metadata.EntityType.GetProperty(property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop is null)
            return BadRequest(new { message = $"La propiedad '{property}' no existe en '{entityName}'." });

        var typedValue = ConvertFilterValue(value, prop.PropertyType);
        if (typedValue is null)
            return BadRequest(new { message = $"No se pudo convertir '{value}' al tipo '{prop.PropertyType.Name}'." });

        var result = await _service.GetByFilterAsync(metadata.EntityType, metadata.ResponseDtoType, prop.Name, typedValue);
        return Ok(result);
    }

    [HttpGet("schema")]
    public IActionResult GetSchema(string entityName)
    {
        var metadata = _metadataProvider.GetMetadata(entityName);
        if (metadata is null)
            return NotFound(new { message = $"Entidad '{entityName}' no registrada." });

        return Ok(new
        {
            entidad = entityName,
            crear = BuildDtoSchema(metadata.CreateDtoType),
            actualizar = BuildDtoSchema(metadata.UpdateDtoType),
            respuesta = BuildDtoSchema(metadata.ResponseDtoType)
        });
    }

    private static List<object> BuildDtoSchema(Type dtoType)
    {
        var properties = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var schema = new List<object>();

        foreach (var prop in properties)
        {
            var field = new Dictionary<string, object>
            {
                ["nombre"] = char.ToLowerInvariant(prop.Name[0]) + prop.Name[1..],
                ["tipo"] = GetFriendlyTypeName(prop.PropertyType),
                ["requerido"] = prop.GetCustomAttribute<RequiredAttribute>() is not null
            };

            var maxLength = prop.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLength is not null)
                field["maxLength"] = maxLength.Length;

            var range = prop.GetCustomAttribute<RangeAttribute>();
            if (range is not null)
            {
                field["min"] = range.Minimum;
                field["max"] = range.Maximum;
            }

            var email = prop.GetCustomAttribute<EmailAddressAttribute>();
            if (email is not null)
                field["formato"] = "email";

            schema.Add(field);
        }

        return schema;
    }

    private static string GetFriendlyTypeName(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying is not null)
            return GetFriendlyTypeName(underlying) + "?";

        if (type == typeof(string)) return "string";
        if (type == typeof(int)) return "int";
        if (type == typeof(long)) return "long";
        if (type == typeof(decimal)) return "decimal";
        if (type == typeof(double)) return "double";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(Guid)) return "guid";
        if (type == typeof(DateTime)) return "datetime";

        if (type.IsEnum) return "enum:" + string.Join("|", Enum.GetNames(type));

        return type.Name.ToLowerInvariant();
    }

    private static object? ConvertFilterValue(string value, Type targetType)
    {
        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            if (underlying == typeof(Guid)) return Guid.Parse(value);
            if (underlying == typeof(int)) return int.Parse(value);
            if (underlying == typeof(bool)) return bool.Parse(value);
            if (underlying == typeof(double)) return double.Parse(value);
            if (underlying == typeof(decimal)) return decimal.Parse(value);
            if (underlying == typeof(DateTime)) return DateTime.Parse(value);
            if (underlying == typeof(string)) return value;
            if (underlying.IsEnum) return Enum.Parse(underlying, value, ignoreCase: true);
            return Convert.ChangeType(value, underlying);
        }
        catch
        {
            return null;
        }
    }
}
