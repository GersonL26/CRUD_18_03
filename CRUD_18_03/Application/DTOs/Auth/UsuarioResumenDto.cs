namespace CRUD_18_03.Application.DTOs.Auth;

public class UsuarioResumenDto
{
    public Guid Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
