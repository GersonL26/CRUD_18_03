using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Producto;

public class CrearProductoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ID de categoría es obligatorio.")]
    public Guid CategoriaId { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
    public decimal Precio { get; set; }

    public bool Activo { get; set; } = true;
}
