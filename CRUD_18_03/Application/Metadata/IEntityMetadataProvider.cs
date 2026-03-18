namespace CRUD_18_03.Application.Metadata;

public interface IEntityMetadataProvider
{
    EntityMetadata? GetMetadata(string entityName);
    IEnumerable<string> GetRegisteredEntityNames();
}
