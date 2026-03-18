namespace CRUD_18_03.Application.DTOs.Supervisor;

public class SupervisorDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public Guid SucursalId { get; set; }
}
