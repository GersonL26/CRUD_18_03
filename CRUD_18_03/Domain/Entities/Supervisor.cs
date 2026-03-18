namespace CRUD_18_03.Domain.Entities;

public class Supervisor : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public Guid SucursalId { get; set; }

    public Sucursal? Sucursal { get; set; }
}
