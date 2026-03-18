namespace CRUD_18_03.Application.Mapping;

public interface IEntityMapper
{
    object MapToNew(object source, Type destinationType);
    void MapToExisting(object source, object destination);
}
