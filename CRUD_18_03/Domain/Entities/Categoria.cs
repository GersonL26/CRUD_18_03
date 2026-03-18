namespace CRUD_18_03.Domain.Entities;

public class Categoria : BaseEntity
{
    public string Descripcion { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
}
