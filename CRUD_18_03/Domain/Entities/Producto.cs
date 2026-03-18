namespace CRUD_18_03.Domain.Entities;

public class Producto : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public Guid CategoriaId { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; } = true;

    public Categoria? Categoria { get; set; }
}
