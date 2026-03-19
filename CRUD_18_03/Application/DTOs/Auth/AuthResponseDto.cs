using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Auth;

public class AuthResponseDto
{
    public Guid UsuarioId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public string Token { get; set; } = string.Empty;
}
