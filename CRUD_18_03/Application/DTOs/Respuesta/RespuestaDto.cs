namespace CRUD_18_03.Application.DTOs.Respuesta;

public class RespuestaDto
{
    public Guid Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int? TiempoUsadoSegundos { get; set; }
    public Guid CandidatoId { get; set; }
    public Guid PreguntaId { get; set; }
    public double? ScoreIA { get; set; }
    public string? FeedbackIA { get; set; }
    public string? BrechasIdentificadas { get; set; }
    public DateTime CreadoEn { get; set; }
    public bool EstaActivo { get; set; }
}
