using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Sucursal;

public class CrearSucursalDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ubicación es obligatoria.")]
    [MaxLength(250)]
    public string Ubicacion { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
