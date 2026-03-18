using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Supervisor;

public class CrearSupervisorDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [MaxLength(250)]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ID de sucursal es obligatorio.")]
    public Guid SucursalId { get; set; }
}
