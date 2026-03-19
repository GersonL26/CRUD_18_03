using CRUD_18_03.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<Candidato> Candidatos => Set<Candidato>();
    public DbSet<Respuesta> Respuestas => Set<Respuesta>();
    public DbSet<ResultadoEvaluacion> ResultadosEvaluacion => Set<ResultadoEvaluacion>();
    public DbSet<SesionEnVivo> SesionesEnVivo => Set<SesionEnVivo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(250);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Rol).HasConversion<int>();
        });

        modelBuilder.Entity<Evaluacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.Tecnologia).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Nivel).HasConversion<int>();
            entity.Property(e => e.Estado).HasConversion<int>();
            entity.HasMany(e => e.Preguntas)
                  .WithOne(p => p.Evaluacion)
                  .HasForeignKey(p => p.EvaluacionId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Candidatos)
                  .WithOne(c => c.Evaluacion)
                  .HasForeignKey(c => c.EvaluacionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Pregunta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Texto).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Tipo).HasConversion<int>();
            entity.Property(e => e.Rubrica).HasMaxLength(1000);
        });

        modelBuilder.Entity<Candidato>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(64);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Respuestas)
                  .WithOne(r => r.Candidato)
                  .HasForeignKey(r => r.CandidatoId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Resultado)
                  .WithOne(r => r.Candidato)
                  .HasForeignKey<ResultadoEvaluacion>(r => r.CandidatoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Respuesta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Contenido).IsRequired();
            entity.Property(e => e.ScoreIA).HasPrecision(5, 2);
            entity.Property(e => e.FeedbackIA).HasMaxLength(2000);
            entity.Property(e => e.BrechasIdentificadas).HasMaxLength(2000);
            entity.HasOne(e => e.Pregunta)
                  .WithMany()
                  .HasForeignKey(e => e.PreguntaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ResultadoEvaluacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ScoreTotal).HasPrecision(5, 2);
            entity.Property(e => e.Recomendacion).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ResumenIA).IsRequired();
            entity.Property(e => e.BrechasDetectadas).HasMaxLength(4000);
            entity.Property(e => e.FortalezasDetectadas).HasMaxLength(4000);
        });

        modelBuilder.Entity<SesionEnVivo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Evaluacion)
                  .WithMany()
                  .HasForeignKey(e => e.EvaluacionId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Candidato)
                  .WithMany()
                  .HasForeignKey(e => e.CandidatoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
