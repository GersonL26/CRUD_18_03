namespace CRUD_18_03.Application.DTOs.Sucursal;

public class SucursalDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
