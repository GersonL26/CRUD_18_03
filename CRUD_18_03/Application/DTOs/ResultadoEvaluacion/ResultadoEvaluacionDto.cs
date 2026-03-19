namespace CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

public class ResultadoEvaluacionDto
{
    public Guid Id { get; set; }
    public double ScoreTotal { get; set; }
    public string Recomendacion { get; set; } = string.Empty;
    public string ResumenIA { get; set; } = string.Empty;
    public string BrechasDetectadas { get; set; } = string.Empty;
    public string FortalezasDetectadas { get; set; } = string.Empty;
    public Guid CandidatoId { get; set; }
    public DateTime GeneradoEn { get; set; }
    public DateTime CreadoEn { get; set; }
    public bool EstaActivo { get; set; }
}
