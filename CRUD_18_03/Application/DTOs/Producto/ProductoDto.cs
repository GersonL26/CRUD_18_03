namespace CRUD_18_03.Application.DTOs.Producto;

public class ProductoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Guid CategoriaId { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
}
