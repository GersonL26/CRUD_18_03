namespace CRUD_18_03.Application.DTOs.Ubicacion;

public class UbicacionDto
{
    public Guid Id { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
}
