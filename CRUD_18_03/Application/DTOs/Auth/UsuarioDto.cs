using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Auth;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public DateTime CreadoEn { get; set; }
    public bool EstaActivo { get; set; }
}

public class ActualizarUsuarioDto
{
    public string? NombreCompleto { get; set; }
    public RolUsuario? Rol { get; set; }
    public bool? EstaActivo { get; set; }
}
