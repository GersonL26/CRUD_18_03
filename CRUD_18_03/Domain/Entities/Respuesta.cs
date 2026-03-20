namespace CRUD_18_03.Domain.Entities;

public class Respuesta : BaseEntity
{
    public string Contenido { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int? TiempoUsadoSegundos { get; set; }
    public Guid CandidatoId { get; set; }
    public Guid PreguntaId { get; set; }

    // Datos del análisis IA (se llenan después)
    public decimal? ScoreIA { get; set; }
    public string? FeedbackIA { get; set; }
    public string? BrechasIdentificadas { get; set; }

    public Candidato? Candidato { get; set; }
    public Pregunta? Pregunta { get; set; }
}
