using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Categoria;

public class CrearCategoriaDto
{
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MaxLength(250)]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código es obligatorio.")]
    [MaxLength(50)]
    public string Codigo { get; set; } = string.Empty;
}
