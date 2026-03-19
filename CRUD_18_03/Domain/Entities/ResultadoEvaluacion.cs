namespace CRUD_18_03.Domain.Entities;

public class ResultadoEvaluacion : BaseEntity
{
    public double ScoreTotal { get; set; }
    public string Recomendacion { get; set; } = string.Empty;
    public string ResumenIA { get; set; } = string.Empty;
    public string BrechasDetectadas { get; set; } = string.Empty;
    public string FortalezasDetectadas { get; set; } = string.Empty;
    public Guid CandidatoId { get; set; }
    public DateTime GeneradoEn { get; set; } = DateTime.UtcNow;

    public Candidato? Candidato { get; set; }
}
