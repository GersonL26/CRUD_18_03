namespace CRUD_18_03.Application.DTOs.Producto;

public class ActualizarProductoDto
{
    public string? Nombre { get; set; }
    public Guid? CategoriaId { get; set; }
    public decimal? Precio { get; set; }
    public bool? Activo { get; set; }
}
