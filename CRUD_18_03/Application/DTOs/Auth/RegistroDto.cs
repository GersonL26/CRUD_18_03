using System.ComponentModel.DataAnnotations;
using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Auth;

public class RegistroDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    [MaxLength(250)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; } = RolUsuario.Candidato;
}
