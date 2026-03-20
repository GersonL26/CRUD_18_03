namespace CRUD_18_03.Domain.Entities;

public class EventoProctoring : BaseEntity
{
    public Guid CandidatoId { get; set; }
    public Guid EvaluacionId { get; set; }
    public string Tipo { get; set; } = string.Empty;       // CopyPaste, TabSwitch, TranscripcionAudio
    public string? Detalle { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Candidato? Candidato { get; set; }
    public Evaluacion? Evaluacion { get; set; }
}
