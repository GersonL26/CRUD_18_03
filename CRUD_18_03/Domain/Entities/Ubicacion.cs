namespace CRUD_18_03.Domain.Entities;

public class Ubicacion : BaseEntity
{
    public string Direccion { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
}
