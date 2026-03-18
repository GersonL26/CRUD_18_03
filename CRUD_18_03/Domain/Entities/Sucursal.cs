namespace CRUD_18_03.Domain.Entities;

public class Sucursal : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
