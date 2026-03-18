using CRUD_18_03.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Sucursal> Sucursales => Set<Sucursal>();
    public DbSet<Supervisor> Supervisores => Set<Supervisor>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Ubicacion).IsRequired().HasMaxLength(250);
        });

        modelBuilder.Entity<Supervisor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Correo).IsRequired().HasMaxLength(250);
            entity.HasOne(e => e.Sucursal)
                  .WithMany()
                  .HasForeignKey(e => e.SucursalId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Precio).HasPrecision(18, 2);
            entity.HasOne(e => e.Categoria)
                  .WithMany()
                  .HasForeignKey(e => e.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Correo).IsRequired().HasMaxLength(250);
        });

        modelBuilder.Entity<Ubicacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Direccion).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Municipio).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Departamento).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Pais).IsRequired().HasMaxLength(150);
        });
    }
}
