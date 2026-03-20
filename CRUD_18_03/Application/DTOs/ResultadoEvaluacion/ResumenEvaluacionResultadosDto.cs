namespace CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

public class ResumenCandidatoDto
{
    public Guid CandidatoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal? ScoreTotal { get; set; }
    public string? Recomendacion { get; set; }
    public bool ResultadoLiberado { get; set; }
    public string? TiempoInvertido { get; set; }
    public DateTime? FechaFinRespuesta { get; set; }
    public DateTime? FechaAnalisis { get; set; }
}

public class ResumenEvaluacionResultadosDto
{
    public Guid EvaluacionId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Tecnologia { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public int TotalCandidatos { get; set; }
    public int CandidatosRespondieron { get; set; }
    public int CandidatosAnalizados { get; set; }
    public decimal? ScorePromedio { get; set; }
    public List<ResumenCandidatoDto> Candidatos { get; set; } = new();
}
