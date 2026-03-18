using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Ubicacion;

public class CrearUbicacionDto
{
    [Required(ErrorMessage = "La direccion es obligatoria.")]
    [MaxLength(250)]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El municipio es obligatorio.")]
    [MaxLength(150)]
    public string Municipio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El departamento de sucursal es obligatorio.")]
    [MaxLength(150)]
    public string Departamento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El pais de sucursal es obligatorio.")]
    [MaxLength(150)]
    public string Pais { get; set; } = string.Empty;
}
