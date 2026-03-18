namespace CRUD_18_03.Application.Metadata;

public class EntityMetadata
{
    public string EntityName { get; }
    public Type EntityType { get; }
    public Type ResponseDtoType { get; }
    public Type CreateDtoType { get; }
    public Type UpdateDtoType { get; }

    public EntityMetadata(
        string entityName,
        Type entityType,
        Type responseDtoType,
        Type createDtoType,
        Type updateDtoType)
    {
        EntityName = entityName;
        EntityType = entityType;
        ResponseDtoType = responseDtoType;
        CreateDtoType = createDtoType;
        UpdateDtoType = updateDtoType;
    }
}
